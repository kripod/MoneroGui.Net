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
using System.Threading.Tasks;
using System.Timers;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class WalletManager : BaseProcessManager, IDisposable
    {
        public event EventHandler Refreshed;
        public event EventHandler<PassphraseRequestedEventArgs> PassphraseRequested;

        public event EventHandler<string> AddressReceived;
        public event EventHandler<BalanceChangingEventArgs> BalanceChanging;
        public event EventHandler<string> SentMoney;

        private static readonly string[] ProcessArgumentsDefault = { "--set_log 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private RpcWebClient RpcWebClient { get; set; }

        private DaemonManager Daemon { get; set; }
        private Paths Paths { get; set; }

        public string Address { get; private set; }
        public Balance Balance { get; private set; }

        private ObservableCollection<Transaction> TransactionsPrivate { get; set; }
        public ReadOnlyObservableCollection<Transaction> Transactions { get; private set; }

        public bool IsWalletFileExistent {
            get { return File.Exists(Paths.FileWalletData); }
        }

        private Timer RefreshTimer { get; set; }

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

            SetProcessArguments();
        }

        private void SetProcessArguments(string passphrase = null)
        {
            ProcessArgumentsExtra = new List<string>(5) {
                "--daemon-address " + RpcWebClient.Host + ":" + RpcWebClient.PortDaemon,
                //"--rpc-bind-ip " + rpcWebClient.Host,
                //"--rpc-bind-port " + rpcWebClient.PortWallet
            };

            if (File.Exists(Paths.FileWalletData)) {
                ProcessArgumentsExtra.Add("--wallet-file=\"" + Paths.FileWalletData + "\"");

            } else {
                var directoryWalletData = Paths.DirectoryWalletData;

                if (!Directory.Exists(directoryWalletData)) Directory.CreateDirectory(directoryWalletData);
                ProcessArgumentsExtra.Add("--generate-new-wallet=\"" + Paths.FileWalletData + "\"");
            }

            if (!string.IsNullOrEmpty(passphrase)) {
                ProcessArgumentsExtra.Add("--password=\"" + passphrase + "\"");
            }
        }

        internal void Start()
        {
            if (TransactionsPrivate == null) {
                TransactionsPrivate = new ObservableCollection<Transaction>();
                Transactions = new ReadOnlyObservableCollection<Transaction>(TransactionsPrivate);
            } else {
                TransactionsPrivate.Clear();
            }

            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());

            if (RefreshTimer != null) RefreshTimer.Dispose();
            RefreshTimer = new Timer(10000);
            RefreshTimer.Elapsed += delegate { Refresh(); };
        }

        private void StartRpcServices()
        {
            // TODO: Make wallet RPC calls work
            //AutoQueryBalanceAsync();
            AutoSaveWalletAsync();
        }

        internal void RequestPassphrase(bool isFirstTime)
        {
            if (PassphraseRequested != null) PassphraseRequested(this, new PassphraseRequestedEventArgs(isFirstTime));
        }

        private async void AutoQueryBalanceAsync()
        {
            while (IsProcessAlive) {
                if (BalanceChanging != null) {
                    var output = await RpcWebClient.JsonQueryDataAsync<Balance>(RpcPortType.Wallet, new GetBalance());
                    BalanceChanging(this, new BalanceChangingEventArgs(output));
                    Balance = output;
                }

                await Task.Delay(10000);
            }
        }

        private async void AutoSaveWalletAsync()
        {
            while (IsProcessAlive) {
                // TODO: Add support for custom intervals
                await Task.Delay(120000);
                Send("save");
            }
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

            if (dataLower.Contains("refresh done")) {
                RefreshTimer.Start();
                if (Refreshed != null) Refreshed(this, EventArgs.Empty);
                return;
            }

            if (BalanceChanging != null && dataLower.Contains("balance")) {
                var match = Regex.Match(dataLower, "balance: ([0-9\\.,]*), unlocked balance: ([0-9\\.,]*)");
                if (match.Success) {
                    var total = double.Parse(match.Groups[1].Value, Helper.InvariantCulture);
                    var spendable = double.Parse(match.Groups[2].Value, Helper.InvariantCulture);

                    var newValue = new Balance(total, spendable);
                    BalanceChanging(this, new BalanceChangingEventArgs(newValue));
                    Balance = newValue;
                }

                return;
            }

            if (SentMoney != null && dataLower.Contains("money successfully sent")) {
                var match = Regex.Match(data, "transaction <([0-9a-z]*)>", RegexOptions.IgnoreCase);
                if (match.Success) {
                    SentMoney(this, match.Groups[1].Value);
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

                    TransactionsPrivate.Add(new Transaction(type, isAmountSpendable, amount, transactionId, TransactionsPrivate.Count + 1));

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
                StartRpcServices();
                return;
            }

            if (AddressReceived != null && (dataLower.Contains("opened wallet: ") || dataLower.Contains("generated new wallet: "))) {
                Address = data.Substring(data.IndexOf(':') + 2);
                AddressReceived(this, Address);
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
                transfers += " " + keyValuePair.Key + " " + keyValuePair.Value;
            }

            var stringFormat = string.IsNullOrWhiteSpace(paymentId) ? "transfer {0}{1}" : "transfer {0}{1} {2}";
            Send(string.Format(Helper.InvariantCulture, stringFormat, mixCount, transfers, paymentId));
        }

        public void Refresh()
        {
            RefreshTimer.Stop();
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
                if (RefreshTimer != null) {
                    RefreshTimer.Dispose();
                    RefreshTimer = null;
                }

                base.Dispose();
            }
        }
    }
}
