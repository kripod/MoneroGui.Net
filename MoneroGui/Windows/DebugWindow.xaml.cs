namespace Jojatekok.MoneroGUI.Windows
{
    public partial class DebugWindow
    {
        public DebugWindow()
        {
            InitializeComponent();

            LogDaemon.DataContext = StaticObjects.LoggerDaemon;
            LogWallet.DataContext = StaticObjects.LoggerWallet;
        }
    }
}
