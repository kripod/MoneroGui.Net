using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class WalletManager : BaseProcessManager, IDisposable
    {
        public event EventHandler Refreshed;
        public event EventHandler<PassphraseRequestedEventArgs> PassphraseRequested;

        public event EventHandler<AddressReceivedEventArgs> AddressReceived;
        public event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        public event EventHandler<BalanceChangingEventArgs> BalanceChanging;
        public event EventHandler<MoneySentEventArgs> SentMoney;

        private static readonly string[] ProcessArgumentsDefault = { "--set_log 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        //private Timer TimerCheckRpcAvailability { get; set; }
        //private Timer TimerQueryBalance { get; set; }
        private Timer TimerRefresh { get; set; }
        private Timer TimerSaveWallet { get; set; }

        private RpcWebClient RpcWebClient { get; set; }

        private DaemonManager Daemon { get; set; }
        private Paths Paths { get; set; }

        public string Address { get; private set; }
        public Balance Balance { get; private set; }

        private ObservableCollection<Transaction> TransactionsPrivate { get; set; }
        public ConcurrentReadOnlyObservableCollection<Transaction> Transactions { get; private set; }

        public bool IsWalletFileExistent {
            get { return File.Exists(Paths.FileWalletData); }
        }

        private string _passphrase;
        public string Passphrase {
            get { return _passphrase; }

            set {
                _passphrase = value;

                KillBaseProcess();

                SetProcessArguments(value);
                Start();
            }
        }

        internal WalletManager(RpcWebClient rpcWebClient, DaemonManager daemon, Paths paths) : base(paths.SoftwareWallet)
        {
            ErrorReceived += Process_ErrorReceived;
            OutputReceived += Process_OutputReceived;

            RpcWebClient = rpcWebClient;

            Daemon = daemon;
            Daemon.BlockchainSynced += Daemon_BlockchainSynced;

            Paths = paths;

            //TimerCheckRpcAvailability = new Timer(delegate { CheckRpcAvailability(); });
            //TimerQueryBalance = new Timer(delegate { QueryBalance(); });
            TimerRefresh = new Timer(delegate { Refresh(); });
            TimerSaveWallet = new Timer(delegate { SaveWallet(); });
        }

        private void SetProcessArguments(string passphrase = null)
        {
            ProcessArgumentsExtra = new List<string>(5) {
                "--daemon-address " + RpcWebClient.Host + ":" + RpcWebClient.PortDaemon,
            };

            if (File.Exists(Paths.FileWalletData)) {
                ProcessArgumentsExtra.Add("--wallet-file \"" + Paths.FileWalletData + "\"");

            } else {
                var directoryWalletData = Paths.DirectoryWalletData;

                if (!Directory.Exists(directoryWalletData)) Directory.CreateDirectory(directoryWalletData);
                ProcessArgumentsExtra.Add("--generate-new-wallet \"" + Paths.FileWalletData + "\"");
                return;
            }

            if (!string.IsNullOrEmpty(passphrase)) {
                ProcessArgumentsExtra.Add("--password \"" + passphrase + "\"");
            }

            // TODO: Enable RPC mode
            //ProcessArgumentsExtra.Add("--rpc-bind-ip " + RpcWebClient.Host);
            //ProcessArgumentsExtra.Add("--rpc-bind-port " + RpcWebClient.PortWallet);
        }

        internal void Start()
        {
            if (ProcessArgumentsExtra == null) SetProcessArguments();

            if (TransactionsPrivate == null) {
                TransactionsPrivate = new ObservableCollection<Transaction>();
                Transactions = new ConcurrentReadOnlyObservableCollection<Transaction>(TransactionsPrivate);
            } else {
                TransactionsPrivate.Clear();
            }

            Debug.Assert(ProcessArgumentsExtra != null, "ProcessArgumentsExtra != null");
            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());

            // Constantly check for the RPC port's activeness
            //TimerCheckRpcAvailability.Change(5000, 1000);
        }

        //private void StartRpcServices()
        //{
        //    QueryAddress();
        //    TimerQueryBalance.Change(0, 10000);
        //    TimerSaveWallet.Change(120000, 120000);
        //}

        internal void RequestPassphrase(bool isFirstTime)
        {
            if (PassphraseRequested != null) PassphraseRequested(this, new PassphraseRequestedEventArgs(isFirstTime));
        }

        //private void CheckRpcAvailability()
        //{
        //    if (Helper.IsPortInUse(RpcWebClient.PortWallet)) {
        //        TimerCheckRpcAvailability.Stop();
        //        StartRpcServices();
        //    }
        //}

        //private void QueryAddress()
        //{
        //    var output = RpcWebClient.JsonQueryData<Address>(RpcPortType.Wallet, new GetAddress());

        //    Address = output.Value;
        //    if (AddressReceived != null) AddressReceived(this, new AddressReceivedEventArgs(Address));
        //}

        //private void QueryBalance()
        //{
        //    var output = RpcWebClient.JsonQueryData<Balance>(RpcPortType.Wallet, new GetBalance());

        //    if (BalanceChanging != null) BalanceChanging(this, new BalanceChangingEventArgs(output));
        //    Balance = output;
        //}

        private void SaveWallet()
        {
            Send("save");
        }

        private void Daemon_BlockchainSynced(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Process_ErrorReceived(object sender, string e)
        {
            var dataLower = e.ToLower(Helper.InvariantCulture);

            if (dataLower.Contains("failed to connect")) {
                // Cannot connect to the daemon

            } else if (dataLower.Contains("failed to generate new wallet")) {
                // Failed to generate a new wallet file

            } else if (dataLower.Contains("invalid password")) {
                // Invalid passphrase
                RequestPassphrase(false);

            } else if (dataLower.Contains("wrong address")) {
                // Invalid send address

            } else if (dataLower.Contains("not enough money")) {
                // Not enough money

            } else if (dataLower.Contains("payment id has invalid format")) {
                // The payment ID needs to be a 64 character string
            }

            // TODO: Handle unexpected errors
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var data = e;
            var dataLower = e.ToLower(Helper.InvariantCulture);

            // <-- Reply methods -->

            if (dataLower.Contains("refresh done") || dataLower.Contains("refresh failed")) {
                TimerRefresh.Change(10000, Timeout.Infinite);
                if (Refreshed != null) Refreshed(this, EventArgs.Empty);
                return;
            }

            if (dataLower.Contains("balance")) {
                var match = Regex.Match(dataLower, "balance: ([0-9\\.,]*), unlocked balance: ([0-9\\.,]*)");
                if (match.Success) {
                    var total = double.Parse(match.Groups[1].Value, Helper.InvariantCulture);
                    var spendable = double.Parse(match.Groups[2].Value, Helper.InvariantCulture);

                    var newValue = new Balance(total, spendable);
                    if (BalanceChanging != null) BalanceChanging(this, new BalanceChangingEventArgs(newValue));
                    Balance = newValue;
                }

                return;
            }

            if (SentMoney != null && dataLower.Contains("money successfully sent")) {
                var match = Regex.Match(data, "transaction <([0-9a-z]*)>", RegexOptions.IgnoreCase);
                if (match.Success) {
                    SentMoney(this, new MoneySentEventArgs(match.Groups[1].Value));
                }

                return;
            }

            // <-- Transaction fetching -->

            // Incoming transaction fetching
            if (dataLower.Contains("transaction")) {
                var match = Regex.Match(data, "height ([0-9]+), transaction <([0-9a-z]+)>, ([a-z]+) ([0-9]+\\.[0-9]+)", RegexOptions.IgnoreCase);
                if (match.Success) {
                    // TODO: Handle block height to get the transaction's timestamp
                    //var blockHeight = ulong.Parse(match.Groups[1].Value, Helper.InvariantCulture);

                    var transactionId = match.Groups[2].Value;
                    var type = match.Groups[3].Value == "received" ? TransactionType.Receive : TransactionType.Send;
                    var amount = double.Parse(match.Groups[4].Value, Helper.InvariantCulture);
                    var isAmountSpendable = type == TransactionType.Receive;

                    var transaction = new Transaction(type, isAmountSpendable, amount, transactionId, TransactionsPrivate.Count + 1);
                    TransactionsPrivate.Add(transaction);
                    if (TransactionReceived != null) TransactionReceived(this, new TransactionReceivedEventArgs(transaction));

                    if (type == TransactionType.Send) {
                        // TODO: Refresh funds' availability more solidly
                        GetAllTransfers();
                    }

                    return;
                }
            }

            // Initial transaction fetching
            var newTransactionMatch = Regex.Match(data, "([0-9]+\\.[0-9]+)[\\s]+([tf])[\\s]+[0-9]+[\\s]+<([0-9a-z]+)>", RegexOptions.IgnoreCase);
            if (newTransactionMatch.Success) {
                var amount = double.Parse(newTransactionMatch.Groups[1].Value, Helper.InvariantCulture);
                var isAmountSpendable = newTransactionMatch.Groups[2].Value == "F";
                var transactionId = newTransactionMatch.Groups[3].Value;

                // TODO: Fetch the transaction's type if possible
                TransactionsPrivate.Add(new Transaction(TransactionType.Unknown, isAmountSpendable, amount, transactionId, TransactionsPrivate.Count + 1));

                return;
            }

            // Clear the list of transactions before they are reloaded
            if (Regex.IsMatch(dataLower, "amount[\\s]+spent")) {
                TransactionsPrivate.Clear();
                return;
            }

            // <-- Initializers -->

            if (dataLower.Contains(" wallet v")) {
                // Startup commands
                GetBalance();
                GetAllTransfers();
                TimerSaveWallet.Change(120000, 120000); // TODO: Handle wallet saving by the RPC
                return;
            }

            if (dataLower.Contains("opened wallet: ") || dataLower.Contains("generated new wallet: ")) {
                Address = data.Substring(data.IndexOf(':') + 2);
                if (AddressReceived != null) AddressReceived(this, new AddressReceivedEventArgs(Address));
                return;
            }

            // <-- Error handler -->

            if (dataLower.Contains("error")) Process_ErrorReceived(this, data);
        }

        private void GetBalance()
        {
            Send("balance");
        }

        private void GetAllTransfers()
        {
            Send("incoming_transfers");
        }

        public void Transfer(Dictionary<string, double> recipients, int mixCount, string paymentId)
        {
            if (recipients == null || recipients.Count == 0) return;

            var transfers = string.Empty;
            foreach (var keyValuePair in recipients) {
                transfers += " " + keyValuePair.Key + " " + keyValuePair.Value.ToString(Helper.InvariantCulture);
            }

            var stringFormat = string.IsNullOrWhiteSpace(paymentId) ? "transfer {0}{1}" : "transfer {0}{1} {2}";
            Send(string.Format(Helper.InvariantCulture, stringFormat, mixCount, transfers, paymentId));
        }

        public void Refresh()
        {
            TimerRefresh.Stop();
            Send("refresh");
        }

        private void Backup(string path)
        {
            if (path == null) {
                path = Paths.DirectoryWalletBackups + DateTime.Now.ToString("yyyy-MM-dd", Helper.InvariantCulture);
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var walletName = Path.GetFileNameWithoutExtension(Paths.FileWalletData);

            var filesToBackup = Directory.GetFiles(Paths.DirectoryWalletData, walletName + "*", SearchOption.TopDirectoryOnly);
            for (var i = filesToBackup.Length - 1; i >= 0; i--) {
                var file = filesToBackup[i];
                Debug.Assert(file != null, "file != null");
                File.Copy(file, Path.Combine(path, Path.GetFileName(file)), true);
            }
        }

        private void Backup()
        {
            Backup(null);
        }

        public Task BackupAsync(string path)
        {
            return Task.Factory.StartNew(() => Backup(path));
        }

        public Task BackupAsync()
        {
            return Task.Factory.StartNew(Backup);
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                base.Dispose();

                //TimerCheckRpcAvailability.Dispose();
                //TimerCheckRpcAvailability = null;

                //TimerQueryBalance.Dispose();
                //TimerQueryBalance = null;

                TimerRefresh.Dispose();
                TimerRefresh = null;

                TimerSaveWallet.Dispose();
                TimerSaveWallet = null;
            }
        }
    }
}
