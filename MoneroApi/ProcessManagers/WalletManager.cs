using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class WalletManager : BaseProcessManager, IBaseProcessManager, IDisposable
    {
        public event EventHandler<PassphraseRequestedEventArgs> PassphraseRequested;

        public event EventHandler<AddressReceivedEventArgs> AddressReceived;
        public event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        public event EventHandler<BalanceChangingEventArgs> BalanceChanging;

        private static readonly string[] ProcessArgumentsDefault = { "--set_log 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private bool IsTransactionReceivedEventEnabled { get; set; }
        private bool IsStartForced { get; set; }

        private Timer TimerCheckRpcAvailability { get; set; }
        private Timer TimerRefresh { get; set; }
        private Timer TimerSaveWallet { get; set; }

        private RpcWebClient RpcWebClient { get; set; }
        private PathSettings PathSettings { get; set; }
        private DaemonManager Daemon { get; set; }

        private readonly ObservableCollection<Transaction> _transactionsPrivate = new ObservableCollection<Transaction>();
        private ObservableCollection<Transaction> TransactionsPrivate {
            get { return _transactionsPrivate; }
        }

        private ConcurrentReadOnlyObservableCollection<Transaction> _transactions;
        public ConcurrentReadOnlyObservableCollection<Transaction> Transactions {
            get { return _transactions; }
            private set { _transactions = value; }
        }

        private string _address;
        public string Address {
            get { return _address; }

            private set {
                _address = value;
                if (AddressReceived != null) AddressReceived(this, new AddressReceivedEventArgs(value));
            }
        }

        private Balance _balance;
        public Balance Balance {
            get { return _balance; }

            private set {
                if (BalanceChanging != null) BalanceChanging(this, new BalanceChangingEventArgs(value));
                _balance = value;
            }
        }

        public bool IsWalletKeysFileExistent {
            get { return File.Exists(PathSettings.FileWalletDataKeys); }
        }

        private string _passphrase;
        public string Passphrase {
            get { return _passphrase; }

            set {
                _passphrase = value;
                Restart();
            }
        }

        internal WalletManager(RpcWebClient rpcWebClient, PathSettings pathSettings, DaemonManager daemon) : base(pathSettings.SoftwareWallet)
        {
            ErrorReceived += Process_ErrorReceived;
            OutputReceived += Process_OutputReceived;

            RpcWebClient = rpcWebClient;
            PathSettings = pathSettings;
            Daemon = daemon;

            Transactions = new ConcurrentReadOnlyObservableCollection<Transaction>(TransactionsPrivate);

            TimerCheckRpcAvailability = new Timer(delegate { CheckRpcAvailability(); });
            TimerRefresh = new Timer(delegate { Refresh(); });
            TimerSaveWallet = new Timer(delegate { SaveWallet(); });
        }

        private void SetProcessArguments()
        {
            var rpcSettings = RpcWebClient.RpcSettings;

            ProcessArgumentsExtra = new List<string>(5) {
                "--daemon-address " + rpcSettings.UrlHost + ":" + rpcSettings.UrlPortDaemon,
            };

            if (IsWalletKeysFileExistent) {
                ProcessArgumentsExtra.Add("--wallet-file \"" + PathSettings.FileWalletData + "\"");

                // Enable RPC mode
                ProcessArgumentsExtra.Add("--rpc-bind-port " + rpcSettings.UrlPortWallet);
                if (rpcSettings.UrlHost != StaticObjects.RpcUrlDefaultLocalhost) {
                    ProcessArgumentsExtra.Add("--rpc-bind-ip " + rpcSettings.UrlHost);
                }

            } else {
                var directoryWalletData = PathSettings.DirectoryWalletData;

                if (!Directory.Exists(directoryWalletData)) Directory.CreateDirectory(directoryWalletData);
                ProcessArgumentsExtra.Add("--generate-new-wallet \"" + PathSettings.FileWalletData + "\"");
            }

            ProcessArgumentsExtra.Add("--password \"" + Passphrase + "\"");
        }

        public void Start()
        {
            if (IsStartForced || IsWalletKeysFileExistent) {
                // Start the wallet normally
                IsStartForced = false;
                StartInternal();

            } else {
                // Let the user set a password for the new wallet being created
                IsStartForced = true;
                RequestPassphrase(true);
            }
        }

        private void StartInternal()
        {
            // <-- Reset variables -->

            SetProcessArguments();

            TransactionsPrivate.Clear();
            IsTransactionReceivedEventEnabled = false;

            Address = null;
            Balance = new Balance(null, null);

            // <-- Start process -->

            Debug.Assert(ProcessArgumentsExtra != null, "ProcessArgumentsExtra != null");
            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());

            // <-- Constantly check for the RPC port's activeness -->

            TimerCheckRpcAvailability.Change(TimerSettings.WalletCheckRpcAvailabilityDueTime, TimerSettings.WalletCheckRpcAvailabilityPeriod);
        }

        private void StartRpcServices()
        {
            QueryAddress();
            TimerRefresh.StartImmediately(TimerSettings.WalletRefreshPeriod);
            TimerSaveWallet.StartOnce(TimerSettings.WalletSaveWalletPeriod);
        }

        public void Stop()
        {
            KillBaseProcess();
        }

        public void Restart()
        {
            Stop();
            StartInternal();
        }

        internal void RequestPassphrase(bool isFirstTime)
        {
            if (PassphraseRequested != null) PassphraseRequested(this, new PassphraseRequestedEventArgs(isFirstTime));
        }

        private void CheckRpcAvailability()
        {
            if (Helper.IsPortInUse(RpcWebClient.RpcSettings.UrlPortWallet)) {
                TimerCheckRpcAvailability.Stop();
                StartRpcServices();
            }
        }

        private void QueryAddress()
        {
            Address = JsonQueryData<AddressValueContainer>(new GetAddress()).Value;
        }

        private void QueryBalance()
        {
            var balance = JsonQueryData<Balance>(new GetBalance());
            if (balance != null) {
                Balance = balance;
                Daemon.IsBlockchainSavable = true;
            }
        }

        private void QueryIncomingTransfers()
        {
            var transactions = JsonQueryData<TransactionListValueContainer>(new GetIncomingTransfers());

            if (transactions != null) {
                var currentTransactionCount = TransactionsPrivate.Count;

                // Update existing transactions
                for (var i = currentTransactionCount - 1; i >= 0; i--) {
                    var transaction = transactions.Value[i];
                    transaction.Number = (uint)(i + 1);
                    // TODO: Add support for detecting transaction type

                    TransactionsPrivate[i] = transaction;
                }

                // Add new transactions
                for (var i = currentTransactionCount; i < transactions.Value.Count; i++) {
                    var transaction = transactions.Value[i];
                    transaction.Number = (uint)(TransactionsPrivate.Count + 1);
                    // TODO: Add support for detecting transaction type

                    TransactionsPrivate.Add(transaction);
                    if (IsTransactionReceivedEventEnabled && TransactionReceived != null) {
                        TransactionReceived(this, new TransactionReceivedEventArgs(transaction));
                    }
                }
            }

            IsTransactionReceivedEventEnabled = true;
        }

        public void Refresh()
        {
            TimerRefresh.Stop();
            QueryBalance();
            QueryIncomingTransfers();
            TimerRefresh.StartOnce(TimerSettings.WalletRefreshPeriod);
        }

        private void SaveWallet()
        {
            JsonQueryData(new SaveWallet());
            TimerSaveWallet.StartOnce(TimerSettings.WalletSaveWalletPeriod);
        }

        public bool SendTransfer(IList<TransferRecipient> recipients, string paymentId, ulong mixCount, ulong fee)
        {
            if (recipients == null || recipients.Count == 0) return false;

            var parameters = new SendTransferParameters(recipients) {
                PaymentId = paymentId,
                MixCount = mixCount,
                Fee = fee
            };

            var output = JsonQueryData<TransactionIdValueContainer>(new SendTransfer(parameters));
            if (output == null) return false;

            ulong amountTotal = 0;
            for (var i = recipients.Count - 1; i >= 0; i--) {
                amountTotal += recipients[i].Amount;
            }
            
            if (TransactionReceived != null) {
                TransactionReceived(this, new TransactionReceivedEventArgs(new Transaction {
                    Type = TransactionType.Send,
                    Amount = amountTotal,
                    TransactionId = output.Value,
                }));
            }

            TimerRefresh.StartImmediately(TimerSettings.WalletRefreshPeriod);
            return true;
        }

        private string Backup(string path = null)
        {
            if (path == null) {
                path = PathSettings.DirectoryWalletBackups + DateTime.Now.ToString("yyyy-MM-dd", Helper.InvariantCulture);
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var walletName = Path.GetFileNameWithoutExtension(PathSettings.FileWalletData);

            var filesToBackup = Directory.GetFiles(PathSettings.DirectoryWalletData, walletName + "*", SearchOption.TopDirectoryOnly);
            for (var i = filesToBackup.Length - 1; i >= 0; i--) {
                var file = filesToBackup[i];
                Debug.Assert(file != null, "file != null");
                File.Copy(file, Path.Combine(path, Path.GetFileName(file)), true);
            }

            return path;
        }

        public Task<string> BackupAsync(string path)
        {
            return Task.Factory.StartNew(() => Backup(path));
        }

        public Task<string> BackupAsync()
        {
            return Task.Factory.StartNew(() => Backup());
        }

        private void Process_ErrorReceived(object sender, string e)
        {
            var dataLower = e.ToLower(Helper.InvariantCulture);

            if (dataLower.Contains("signature missmatch")) {
                // Invalid passphrase
                RequestPassphrase(false);
            }
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var data = e;
            var dataLower = e.ToLower(Helper.InvariantCulture);

            if (dataLower.Contains("wallet has been generated")) {
                // Restart in RPC mode after generating a new wallet
                Restart();
            }

            // Error handler
            if (dataLower.Contains("error")) Process_ErrorReceived(this, data);
        }

        private T JsonQueryData<T>(JsonRpcRequest jsonRpcRequest) where T : class
        {
            var output = RpcWebClient.JsonQueryData<T>(RpcPortType.Wallet, jsonRpcRequest);
            var rpcResponse = output as RpcResponse;
            if (rpcResponse == null || rpcResponse.Status == RpcResponseStatus.Ok) {
                return output;
            }

            return null;
        }

        private object JsonQueryData(JsonRpcRequest jsonRpcRequest)
        {
            return JsonQueryData<object>(jsonRpcRequest);
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private new void Dispose(bool disposing)
        {
            if (disposing) {
                TimerCheckRpcAvailability.Dispose();
                TimerCheckRpcAvailability = null;

                TimerRefresh.Dispose();
                TimerRefresh = null;

                TimerSaveWallet.Dispose();
                TimerSaveWallet = null;

                base.Dispose(false);
            }
        }
    }
}
