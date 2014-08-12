using Jojatekok.MoneroAPI;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class DebugWindow
    {
        private static readonly MoneroClient MoneroClient = StaticObjects.MoneroClient;

        public DebugWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();

            ConsoleDaemon.DataContext = StaticObjects.LoggerDaemon;
            ConsoleAccountManager.DataContext = StaticObjects.LoggerAccountManager;
        }

        private void ConsoleDaemon_SendRequested(object sender, string e)
        {
            MoneroClient.Daemon.Send(e);
        }

        private void ConsoleAccountManager_SendRequested(object sender, string e)
        {
            MoneroClient.AccountManager.Send(e);
        }
    }
}
