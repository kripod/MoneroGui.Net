using Jojatekok.MoneroAPI;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class MainWindow : IDisposable
    {
        private bool IsDisposeInProgress { get; set; }

        private static readonly MoneroClient MoneroClient = StaticObjects.MoneroClient;
        private static readonly Logger LoggerDaemon = StaticObjects.LoggerDaemon;
        private static readonly Logger LoggerWallet = StaticObjects.LoggerWallet;

        public static readonly RoutedCommand ExitCommand = new RoutedCommand();
        public static readonly RoutedCommand OptionsCommand = new RoutedCommand();
        public static readonly RoutedCommand ShowDebugWindowCommand = new RoutedCommand();
        public static readonly RoutedCommand ShowAboutWindowCommand = new RoutedCommand();

        private DebugWindow DebugWindow { get; set; }

        public MainWindow()
        {
            Icon = StaticObjects.ApplicationIcon;

            InitializeComponent();

            MoneroClient.Daemon.OnLogMessage += Daemon_OnLogMessage;
            MoneroClient.Daemon.SyncStatusChanged += Daemon_SyncStatusChanged;
            MoneroClient.Daemon.ConnectionCountChanged += Daemon_ConnectionCountChanged;

            MoneroClient.Wallet.OnLogMessage += Wallet_OnLogMessage;
            MoneroClient.Wallet.AddressReceived += Wallet_AddressReceived;
            MoneroClient.Wallet.BalanceChanged += Wallet_BalanceChanged;

            OverviewView.ViewModel.TransactionDataSource = MoneroClient.Wallet.Transactions;
            TransactionsView.ViewModel.DataSource = MoneroClient.Wallet.Transactions;

            MoneroClient.Start();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!IsDisposeInProgress) {
                Task.Factory.StartNew(Dispose);
                BusyIndicator.IsBusy = true;
            }

            e.Cancel = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new OptionsWindow(this).ShowDialog();
        }

        private void ShowDebugWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DebugWindow == null) {
                DebugWindow = new DebugWindow();
                DebugWindow.Closed += delegate { DebugWindow = null; };
                DebugWindow.Show();

            } else {
                DebugWindow.Activate();
            }
        }

        private void ShowAboutWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new AboutWindow(this).ShowDialog();
        }

        private static void Daemon_OnLogMessage(object sender, string e)
        {
            LoggerDaemon.Log(e);
        }

        private void Daemon_SyncStatusChanged(object sender, SyncStatusChangedEventArgs e)
        {
            var statusBarViewModel = StatusBar.ViewModel;

            string timeRemainingText;
            if (e.TimeRemainingText == "days") {
                timeRemainingText = Properties.Resources.StatusBarSyncTextDays;
            } else {
                timeRemainingText = e.TimeRemainingText;
            }

            statusBarViewModel.SyncBarText = string.Format(Helper.InvariantCulture,
                                                           Properties.Resources.StatusBarSyncTextMain,
                                                           e.BlocksRemaining,
                                                           e.TimeRemainingValue,
                                                           timeRemainingText);

            statusBarViewModel.BlocksTotal = e.BlocksTotal;
            statusBarViewModel.BlocksDownloaded = e.BlocksDownloaded;
        }

        private void Daemon_ConnectionCountChanged(object sender, byte e)
        {
            StatusBar.ViewModel.ConnectionCount = e;
        }

        private static void Wallet_OnLogMessage(object sender, string e)
        {
            LoggerWallet.Log(e);
        }

        private void Wallet_AddressReceived(object sender, string e)
        {
            OverviewView.ViewModel.Address = e;
        }

        private void Wallet_BalanceChanged(object sender, Balance e)
        {
            var overviewViewModel = OverviewView.ViewModel;
            overviewViewModel.BalanceSpendable = e.Spendable;
            overviewViewModel.BalanceUnconfirmed = e.Unconfirmed;

            var sendCoinsViewModel = SendCoinsView.ViewModel;
            sendCoinsViewModel.CoinBalance = e.Spendable;
            sendCoinsViewModel.IsSendingEnabled = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposeInProgress) {
                IsDisposeInProgress = true;
                
                if (MoneroClient != null) {
                    MoneroClient.Dispose();
                }

                Dispatcher.Invoke(Application.Current.Shutdown);
            }
        }
    }
}
