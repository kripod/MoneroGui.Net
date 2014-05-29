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

        internal static MoneroClient MoneroClient { get; set; }

        public static readonly RoutedCommand ExitCommand = new RoutedCommand();

        public MainWindow()
        {
            MoneroClient = new MoneroClient();

            InitializeComponent();

            MoneroClient.Daemon.SyncStatusChanged += Daemon_SyncStatusChanged;
            MoneroClient.Daemon.ConnectionCountChanged += Daemon_ConnectionCountChanged;

            MoneroClient.Wallet.AddressReceived += Wallet_AddressReceived;
            MoneroClient.Wallet.BalanceChanged += Wallet_BalanceChanged;
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

        private void Daemon_SyncStatusChanged(object sender, SyncStatusChangedEventArgs e)
        {
            var statusBarViewModel = StatusBar.ViewModel;
            statusBarViewModel.SyncBarText = e.StatusText;
            statusBarViewModel.BlocksTotal = e.BlocksTotal;
            statusBarViewModel.BlocksDownloaded = e.BlocksDownloaded;
        }

        private void Daemon_ConnectionCountChanged(object sender, byte e)
        {
            StatusBar.ViewModel.ConnectionCount = e;
        }

        private void Wallet_AddressReceived(object sender, string e)
        {
            Overview.ViewModel.Address = e;
        }

        private void Wallet_BalanceChanged(object sender, Balance e)
        {
            var overviewViewModel = Overview.ViewModel;
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
                    MoneroClient = null;
                }

                Dispatcher.Invoke(Application.Current.Shutdown);
            }
        }
    }
}
