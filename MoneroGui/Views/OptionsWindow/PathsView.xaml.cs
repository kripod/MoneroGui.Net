namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class PathsView
    {
        public PathsView()
        {
            InitializeComponent();

#if DEBUG
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
#endif

            // Load settings
            var pathSettings = SettingsManager.Paths;
            PathSelectorViewFileWalletData.SelectedPath = pathSettings.FileWalletData;
            PathSelectorViewDirectoryWalletBackups.SelectedPath = pathSettings.DirectoryWalletBackups;
            PathSelectorViewSoftwareDaemon.SelectedPath = pathSettings.SoftwareDaemon;
            PathSelectorViewSoftwareWallet.SelectedPath = pathSettings.SoftwareWallet;
            PathSelectorViewSoftwareMiner.SelectedPath = pathSettings.SoftwareMiner;
        }

        public void ApplySettings()
        {
            var pathSettings = SettingsManager.Paths;
            pathSettings.FileWalletData = PathSelectorViewFileWalletData.SelectedPath;
            pathSettings.DirectoryWalletBackups = PathSelectorViewDirectoryWalletBackups.SelectedPath;
            pathSettings.SoftwareDaemon = PathSelectorViewSoftwareDaemon.SelectedPath;
            pathSettings.SoftwareWallet = PathSelectorViewSoftwareWallet.SelectedPath;
            pathSettings.SoftwareMiner = PathSelectorViewSoftwareMiner.SelectedPath;
        }
    }
}
