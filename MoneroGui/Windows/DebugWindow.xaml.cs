using Jojatekok.MoneroAPI;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class DebugWindow
    {
        private static readonly MoneroClient MoneroClient = StaticObjects.MoneroClient;

        public DebugWindow()
        {
            Icon = StaticObjects.ApplicationIcon;

            InitializeComponent();

            ConsoleDaemon.DataContext = StaticObjects.LoggerDaemon;
            ConsoleWallet.DataContext = StaticObjects.LoggerWallet;
        }

        private void ConsoleDaemon_SendRequested(object sender, string e)
        {
            MoneroClient.Daemon.Send(e);
        }

        private void ConsoleWallet_SendRequested(object sender, string e)
        {
            MoneroClient.Wallet.Send(e);
        }
    }
}
