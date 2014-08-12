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
            PathSelectorFileAccountData.Filter = Properties.Resources.TextFilterAccountFiles + "|" + Properties.Resources.TextFilterAllFiles;
            PathSelectorSoftwareDaemon.Filter = softwareFilter;
            PathSelectorSoftwareAccountManager.Filter = softwareFilter;

            // Load settings
            var pathSettings = SettingsManager.Paths;
            PathSelectorDirectoryDaemonData.SelectedPath = pathSettings.DirectoryDaemonData;
            PathSelectorFileAccountData.SelectedPath = pathSettings.FileAccountData;
            PathSelectorDirectoryAccountBackups.SelectedPath = pathSettings.DirectoryAccountBackups;
            PathSelectorSoftwareDaemon.SelectedPath = pathSettings.SoftwareDaemon;
            PathSelectorSoftwareAccountManager.SelectedPath = pathSettings.SoftwareAccountManager;
        }

        public void ApplySettings()
        {
            var pathSettings = SettingsManager.Paths;
            pathSettings.DirectoryDaemonData = PathSelectorDirectoryDaemonData.SelectedPath;
            pathSettings.FileAccountData = PathSelectorFileAccountData.SelectedPath;
            pathSettings.DirectoryAccountBackups = PathSelectorDirectoryAccountBackups.SelectedPath;
            pathSettings.SoftwareDaemon = PathSelectorSoftwareDaemon.SelectedPath;
            pathSettings.SoftwareAccountManager = PathSelectorSoftwareAccountManager.SelectedPath;
        }
    }
}
