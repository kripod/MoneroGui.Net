using Jojatekok.MoneroAPI.Extensions;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class DebugWindow
    {
        private static readonly MoneroProcessManager MoneroProcessManager = StaticObjects.MoneroProcessManager;

        public DebugWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();

            ConsoleDaemon.DataContext = StaticObjects.LoggerDaemon;
            ConsoleAccountManager.DataContext = StaticObjects.LoggerAccountManager;
        }

        private void ConsoleDaemon_SendRequested(object sender, string e)
        {
            MoneroProcessManager.Daemon.SendConsoleCommand(e);
        }

        private void ConsoleAccountManager_SendRequested(object sender, string e)
        {
            MoneroProcessManager.AccountManager.SendConsoleCommand(e);
        }
    }
}
