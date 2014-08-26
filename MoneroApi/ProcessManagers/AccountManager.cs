using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses;
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
    public class AccountManager : BaseRpcProcessManager, IDisposable
    {
        public event EventHandler<PassphraseRequestedEventArgs> PassphraseRequested;

        public event EventHandler<AddressReceivedEventArgs> AddressReceived;
        public event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        public event EventHandler<BalanceChangingEventArgs> BalanceChanging;

        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private bool IsTransactionReceivedEventEnabled { get; set; }
        private bool IsStartForced { get; set; }

        private Timer TimerRefresh { get; set; }
        private Timer TimerSaveAccount { get; set; }

        private RpcWebClient RpcWebClient { get; set; }
        private PathSettings PathSettings { get; set; }
        private DaemonManager Daemon { get; set; }

        private readonly ObservableCollection<Transaction> _transactionsPrivate = new ObservableCollection<Transaction>();
        private ObservableCollection<Transaction> TransactionsPrivate {
            get { return _transactionsPrivate; }
        }

        public ConcurrentReadOnlyObservableCollection<Transaction> Transactions { get; private set; }

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

        private bool IsAccountKeysFileExistent {
            get { return File.Exists(PathSettings.FileAccountDataKeys); }
        }

        private string _passphrase;
        public string Passphrase {
            get { return _passphrase; }

            set {
                _passphrase = value;
                Restart();
            }
        }

        internal AccountManager(RpcWebClient rpcWebClient, PathSettings pathSettings, DaemonManager daemon) : base(pathSettings.SoftwareAccountManager, rpcWebClient, rpcWebClient.RpcSettings.UrlPortAccountManager)
        {
            Exited += Process_Exited;
            RpcAvailabilityChanged += Process_RpcAvailabilityChanged;

            RpcWebClient = rpcWebClient;
            PathSettings = pathSettings;
            Daemon = daemon;

            Transactions = new ConcurrentReadOnlyObservableCollection<Transaction>(TransactionsPrivate);

            TimerRefresh = new Timer(delegate { RequestRefresh(); });
            TimerSaveAccount = new Timer(delegate { RequestSaveAccount(); });
        }

        private void SetProcessArguments()
        {
            var rpcSettings = RpcWebClient.RpcSettings;

            ProcessArgumentsExtra = new List<string>(5) {
                "--daemon-address " + rpcSettings.UrlHost + ":" + rpcSettings.UrlPortDaemon,
                "--password \"" + Passphrase + "\"",
                "--rpc-bind-port " + rpcSettings.UrlPortAccountManager
            };

            if (rpcSettings.UrlHost != StaticObjects.RpcUrlDefaultLocalhost) {
                ProcessArgumentsExtra.Add("--rpc-bind-ip " + rpcSettings.UrlHost);
            }

            if (IsAccountKeysFileExistent) {
                // Load existing account
                ProcessArgumentsExtra.Add("--wallet-file \"" + PathSettings.FileAccountData + "\"");

            } else {
                // Create new account
                var directoryAccountData = PathSettings.DirectoryAccountData;

                if (!Directory.Exists(directoryAccountData)) Directory.CreateDirectory(directoryAccountData);
                ProcessArgumentsExtra.Add("--generate-new-wallet \"" + PathSettings.FileAccountData + "\"");
            }
        }

        public void Start()
        {
            if (IsStartForced || IsAccountKeysFileExistent) {
                // Start the account normally
                IsStartForced = false;
                StartInternal();

            } else {
                // Let the user set a password for the new account being created
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

        private void QueryAddress()
        {
            Address = JsonPostData<AddressValueContainer>(new QueryAddress()).Result.Value;
        }

        public string QueryKey(QueryKeyParameters.KeyType keyType)
        {
            var key = JsonPostData<KeyValueContainer>(new QueryKey(keyType)).Result;
            return key != null ? key.Value : null;
        }

        private void QueryBalance()
        {
            var balance = JsonPostData<Balance>(new QueryBalance()).Result;
            if (balance != null) {
                Balance = balance;
                Daemon.IsBlockchainSavable = true;
            }
        }

        private void QueryIncomingTransfers()
        {
            var transactions = JsonPostData<TransactionListValueContainer>(new QueryIncomingTransfers()).Result;

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

        private void RequestPassphrase(bool isFirstTime)
        {
            if (PassphraseRequested != null) PassphraseRequested(this, new PassphraseRequestedEventArgs(isFirstTime));
        }

        private void RequestRefresh()
        {
            TimerRefresh.Stop();
            QueryBalance();
            QueryIncomingTransfers();
            TimerRefresh.StartOnce(TimerSettings.AccountRefreshPeriod);
        }

        private void RequestSaveAccount()
        {
            JsonPostData(new RequestSaveAccount());
            TimerSaveAccount.StartOnce(TimerSettings.AccountSaveAccountPeriod);
        }

        public bool SendTransferSplit(IList<TransferRecipient> recipients, string paymentId, ulong mixCount, ulong fee)
        {
            if (recipients == null || recipients.Count == 0) return false;

            var parameters = new SendTransferSplitParameters(recipients) {
                PaymentId = paymentId,
                MixCount = mixCount,
                Fee = fee
            };

            var output = JsonPostData<TransactionIdListValueContainer>(new SendTransferSplit(parameters));
            if (output == null) return false;

            ulong amountTotal = 0;
            for (var i = recipients.Count - 1; i >= 0; i--) {
                amountTotal += recipients[i].Amount;
            }
            
            if (TransactionReceived != null) {
                TransactionReceived(this, new TransactionReceivedEventArgs(new Transaction {
                    Type = TransactionType.Send,
                    Amount = amountTotal
                }));
            }

            TimerRefresh.StartImmediately(TimerSettings.AccountRefreshPeriod);
            return true;
        }

        private string Backup(string path = null)
        {
            if (path == null) {
                path = PathSettings.DirectoryAccountBackups + DateTime.Now.ToString("yyyy-MM-dd", Helper.InvariantCulture);
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var accountName = Path.GetFileNameWithoutExtension(PathSettings.FileAccountData);

            var filesToBackup = Directory.GetFiles(PathSettings.DirectoryAccountData, accountName + "*", SearchOption.TopDirectoryOnly);
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

        private void Process_Exited(object sender, ProcessExitedEventArgs e)
        {
            switch (e.ExitCode) {
                case 1:
                    // Unknown error
                    Restart();
                    break;

                case 10:
                    // Invalid passphrase
                    RequestPassphrase(false);
                    break;
            }
        }

        private void Process_RpcAvailabilityChanged(object sender, EventArgs e)
        {
            if (IsRpcAvailable) {
                QueryAddress();
                TimerRefresh.StartImmediately(TimerSettings.AccountRefreshPeriod);
                TimerSaveAccount.StartOnce(TimerSettings.AccountSaveAccountPeriod);

            } else {
                TimerRefresh.Stop();
                TimerSaveAccount.Stop();
            }
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                TimerRefresh.Dispose();
                TimerRefresh = null;

                TimerSaveAccount.Dispose();
                TimerSaveAccount = null;

                // Safe shutdown
                JsonPostData(new RequestExit());

                base.Dispose();
            }
        }
    }
}
