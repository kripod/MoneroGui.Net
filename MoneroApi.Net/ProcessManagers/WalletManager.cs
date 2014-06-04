using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class WalletManager : BaseProcessManager, IDisposable
    {
        public event EventHandler Refreshed;
        public event EventHandler<string> AddressReceived;
        public event EventHandler<Balance> BalanceChanged;
        public event EventHandler<string> SentMoney;

        private DaemonManager Daemon { get; set; }
        private Paths Paths { get; set; }
        private List<string> ProcessArguments { get; set; }

        public string Address { get; private set; }
        public Balance Balance { get; private set; }

        private ObservableCollection<Transaction> TransactionsPrivate { get; set; }
        public ReadOnlyObservableCollection<Transaction> Transactions { get; private set; }

        private Timer RefreshTimer { get; set; }

        internal WalletManager(DaemonManager daemon, Paths paths, string password = null) : base(paths.SoftwareWallet)
        {
            ErrorReceived += Process_ErrorReceived;
            OutputReceived += Process_OutputReceived;

            Daemon = daemon;
            Daemon.RpcInitialized += Daemon_RpcInitialized;

            Paths = paths;

            ProcessArguments = new List<string>(2);

            if (File.Exists(Paths.FileWalletData)) {
                ProcessArguments.Add("--wallet-file=\"" + Paths.FileWalletData + "\"");

            } else {
                var directoryWalletData = Path.GetDirectoryName(Paths.FileWalletData);
                Debug.Assert(directoryWalletData != null, "directoryWalletData != null");

                if (!Directory.Exists(directoryWalletData)) Directory.CreateDirectory(directoryWalletData);
                ProcessArguments.Add("--generate-new-wallet=\"" + Paths.FileWalletData + "\"");
            }

            if (!string.IsNullOrWhiteSpace(password)) {
                ProcessArguments.Add("--password=\"" + password + "\"");
            }

            TransactionsPrivate = new ObservableCollection<Transaction>();
            Transactions = new ReadOnlyObservableCollection<Transaction>(TransactionsPrivate);
        }

        public void Start()
        {
            StartProcess(ProcessArguments.ToArray());

            RefreshTimer = new Timer(10000);
            RefreshTimer.Elapsed += delegate { Refresh(); };
        }

        private void Daemon_RpcInitialized(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Process_ErrorReceived(object sender, string e)
        {
            if (e.Contains("failed to connect")) {
                // Cannot connect to the daemon

            } else if (e.Contains("failed to generate new wallet")) {
                // Failed to generate a new wallet file

            } else if (e.Contains("invalid password")) {
                // Invalid password

            } else if (e.Contains("wrong address")) {
                // Invalid send address

            } else if (e.Contains("not enough money")) {
                // Not enough money

            } else if (e.Contains("payment id has invalid format")) {
                // The payment ID needs to be a 64 character string
            }

            // TODO: Handle unexpected errors
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var data = e.ToLower(Helper.InvariantCulture);

            // <-- Reply methods -->

            if (data.Contains("refresh done")) {
                RefreshTimer.Start();
                if (Refreshed != null) Refreshed(this, EventArgs.Empty);
                return;
            }

            if (BalanceChanged != null && data.Contains("balance")) {
                var match = Regex.Match(data, "balance: ([0-9\\.,]*), unlocked balance: ([0-9\\.,]*)");
                if (match.Success) {
                    var total = double.Parse(match.Groups[1].Value, Helper.InvariantCulture);
                    var spendable = double.Parse(match.Groups[2].Value, Helper.InvariantCulture);

                    Balance = new Balance(total, spendable);
                    BalanceChanged(this, Balance);
                }

                return;
            }

            if (SentMoney != null && data.Contains("money successfully sent")) {
                var match = Regex.Match(data, "transaction <([0-9a-z]*)>");
                if (match.Success) {
                    SentMoney(this, match.Groups[1].Value);
                }

                return;
            }

            // <-- Transaction fetching -->

            // Incoming transaction fetching
            if (data.Contains("transaction")) {
                var match = Regex.Match(data, "height ([0-9]+), transaction <([0-9a-z]+)>, ([a-z]+) ([0-9]+\\.[0-9]+)");
                if (match.Success) {
                    // TODO: Handle block height to get the transaction's timestamp
                    //var blockHeight = ulong.Parse(match.Groups[1].Value, Helper.InvariantCulture);

                    var transactionId = match.Groups[2].Value;
                    var type = match.Groups[3].Value == "received" ? TransactionType.Receive : TransactionType.Send;
                    var amount = double.Parse(match.Groups[4].Value, Helper.InvariantCulture);
                    var isAmountSpendable = type == TransactionType.Receive;

                    TransactionsPrivate.Add(new Transaction(type, isAmountSpendable, amount, transactionId, TransactionsPrivate.Count + 1));

                    if (type == TransactionType.Send) {
                        // TODO: Refresh funds' availability
                    }

                    return;
                }
            }

            // Initial transaction fetching
            var newTransactionMatch = Regex.Match(data, "([0-9]+\\.[0-9]+)[\\s]+([tf])[\\s]+[0-9]+[\\s]+<([0-9a-z]+)>");
            if (newTransactionMatch.Success) {
                var amount = double.Parse(newTransactionMatch.Groups[1].Value, Helper.InvariantCulture);
                var isAmountSpendable = newTransactionMatch.Groups[2].Value == "f";
                var transactionId = newTransactionMatch.Groups[3].Value;

                // TODO: Fetch the transaction's type if possible
                TransactionsPrivate.Add(new Transaction(TransactionType.Unknown, isAmountSpendable, amount, transactionId, TransactionsPrivate.Count + 1));

                return;
            }

            // Clear the list of transactions before they are reloaded
            if (Regex.IsMatch(data, "amount[\\s]+spent")) {
                TransactionsPrivate.Clear();
                return;
            }

            // <-- Initializers -->

            if (data.Contains(" wallet v")) {
                // Startup commands
                Refresh();
                GetBalance();
                GetAllTransfers();
                return;
            }

            if (AddressReceived != null && (data.Contains("opened wallet: ") || data.Contains("generated new wallet: "))) {
                Address = data.Substring(data.IndexOf(':') + 2);
                AddressReceived(this, Address);
                return;
            }

            // <-- Error handler -->

            if (data.Contains("error")) {
                Process_ErrorReceived(this, data);
            }
        }

        private void GetBalance()
        {
            Send("balance");
        }

        private void GetAllTransfers()
        {
            Send("incoming_transfers");
        }

        public void Transfer(string address, double amount, int mixCount = 0, string paymentId = null)
        {
            Send(string.IsNullOrEmpty(paymentId) ?
                 string.Format(Helper.InvariantCulture, "transfer {0} {1} {2}", mixCount, address, amount) :
                 string.Format(Helper.InvariantCulture, "transfer {0} {1} {2} {3}", mixCount, address, amount, paymentId)
            );
        }

        public void Transfer(Dictionary<string, double> recipients, int mixCount = 0)
        {
            var transfers = string.Empty;
            foreach (var keyValuePair in recipients) {
                transfers += " " + keyValuePair.Key + " " + keyValuePair.Value;
            }

            Send(string.Format(Helper.InvariantCulture, "transfer {0}{1}", mixCount, transfers));
        }

        public void Refresh()
        {
            RefreshTimer.Stop();
            Send("refresh");
        }

        public void Backup(string path = null)
        {
            if (path == null) {
                path = Paths.DirectoryWalletBackups + DateTime.Now.ToString("yyyy-MM-dd");
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var walletName = Path.GetFileNameWithoutExtension(Paths.FileWalletData);
            var directoryWalletData = Path.GetDirectoryName(Paths.FileWalletData);
            Debug.Assert(directoryWalletData != null, "directoryWalletData != null");

            var filesToBackup = Directory.GetFiles(directoryWalletData, walletName + "*");
            for (var i = filesToBackup.Length - 1; i >= 0; i--) {
                var file = filesToBackup[i];
                Debug.Assert(file != null, "file != null");
                File.Copy(file, Path.Combine(path, Path.GetFileName(file)), true);
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
                if (RefreshTimer != null) {
                    RefreshTimer.Dispose();
                    RefreshTimer = null;
                }

                base.Dispose();
            }
        }
    }
}
