using System.ComponentModel;

namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class PathsView : ISettingsView
    {
        public PathsView()
        {
            InitializeComponent();

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
#endif

            var softwareFilter = Properties.Resources.TextFilterExecutableFiles + "|" + Properties.Resources.TextFilterAllFiles;
            PathSelectorFileWalletData.Filter = Properties.Resources.TextFilterWalletFiles + "|" + Properties.Resources.TextFilterAllFiles;
            PathSelectorSoftwareDaemon.Filter = softwareFilter;
            PathSelectorSoftwareWallet.Filter = softwareFilter;

            // Load settings
            var pathSettings = SettingsManager.Paths;
            PathSelectorDirectoryDaemonData.SelectedPath = pathSettings.DirectoryDaemonData;
            PathSelectorFileWalletData.SelectedPath = pathSettings.FileWalletData;
            PathSelectorDirectoryWalletBackups.SelectedPath = pathSettings.DirectoryWalletBackups;
            PathSelectorSoftwareDaemon.SelectedPath = pathSettings.SoftwareDaemon;
            PathSelectorSoftwareWallet.SelectedPath = pathSettings.SoftwareWallet;
        }

        public void ApplySettings()
        {
            var pathSettings = SettingsManager.Paths;
            pathSettings.DirectoryDaemonData = PathSelectorDirectoryDaemonData.SelectedPath;
            pathSettings.FileWalletData = PathSelectorFileWalletData.SelectedPath;
            pathSettings.DirectoryWalletBackups = PathSelectorDirectoryWalletBackups.SelectedPath;
            pathSettings.SoftwareDaemon = PathSelectorSoftwareDaemon.SelectedPath;
            pathSettings.SoftwareWallet = PathSelectorSoftwareWallet.SelectedPath;
        }
    }
}
