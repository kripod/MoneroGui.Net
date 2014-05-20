using MoneroClient.ProcessManagers;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MoneroClient
{
    public partial class MainWindow : IDisposable
    {
        private bool IsDisposeInProgress { get; set; }

        public DaemonManager DaemonManager { get; private set; }

        public static readonly RoutedCommand ExitCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();

            DaemonManager = new DaemonManager();
            DaemonManager.SyncStatusChanged += DaemonManager_SyncStatusChanged;
            DaemonManager.ConnectionCountChanged += DaemonManager_ConnectionCountChanged;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!IsDisposeInProgress) {
                Task.Factory.StartNew(Dispose);
                IsEnabled = false;

                // TODO: Show a dialog of an indeterminate ProgressBar until all the processes are killed
            }

            e.Cancel = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void DaemonManager_SyncStatusChanged(object sender, SyncStatusChangedEventArgs e)
        {
            var statusBarViewModel = StatusBar.ViewModel;
            statusBarViewModel.SyncBarText = e.StatusText;
            statusBarViewModel.BlocksTotal = e.BlocksTotal;
            statusBarViewModel.BlocksDownloaded = e.BlocksDownloaded;
        }

        private void DaemonManager_ConnectionCountChanged(object sender, byte e)
        {
            StatusBar.ViewModel.ConnectionCount = e;
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
                
                if (DaemonManager != null) {
                    DaemonManager.Dispose();
                    DaemonManager = null;
                }

                Dispatcher.Invoke(Application.Current.Shutdown);
            }
        }
    }
}
