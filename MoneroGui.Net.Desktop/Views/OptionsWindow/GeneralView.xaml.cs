using IWshRuntimeLibrary;
using System.ComponentModel;
using System.Diagnostics;
using File = System.IO.File;

namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class GeneralView : ISettingsView
    {
        public GeneralView()
        {
            InitializeComponent();

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
#endif

            // Load settings
            var generalSettings = SettingsManager.General;
            CheckBoxIsUpdateCheckEnabled.IsChecked = generalSettings.IsUpdateCheckEnabled;
            CheckBoxIsUpdateCheckForTestBuildsEnabled.IsChecked = generalSettings.IsUpdateCheckForTestBuildsEnabled;
            CheckBoxIsStartableOnSystemLogin.IsChecked = generalSettings.IsStartableOnSystemLogin;
            CheckBoxIsUriAssociationCheckEnabled.IsChecked = generalSettings.IsUriAssociationCheckEnabled;
            CheckBoxIsSafeShutdownEnabled.IsChecked = generalSettings.IsSafeShutdownEnabled;
        }

        public void ApplySettings()
        {
            Debug.Assert(CheckBoxIsUpdateCheckEnabled.IsChecked != null, "CheckBoxIsUpdateCheckEnabled.IsChecked != null");
            Debug.Assert(CheckBoxIsUpdateCheckForTestBuildsEnabled.IsChecked != null, "CheckBoxIsUpdateCheckForTestBuildsEnabled.IsChecked != null");
            Debug.Assert(CheckBoxIsStartableOnSystemLogin.IsChecked != null, "CheckBoxStartableOnSystemLogin.IsChecked != null");
            Debug.Assert(CheckBoxIsUriAssociationCheckEnabled.IsChecked != null, "CheckBoxIsUriAssociationCheckEnabled.IsChecked != null");
            Debug.Assert(CheckBoxIsSafeShutdownEnabled.IsChecked != null, "CheckBoxSafeShutdownEnabled.IsChecked != null");

            var generalSettings = SettingsManager.General;
            generalSettings.IsUpdateCheckEnabled = CheckBoxIsUpdateCheckEnabled.IsChecked.Value;
            generalSettings.IsUpdateCheckForTestBuildsEnabled = CheckBoxIsUpdateCheckForTestBuildsEnabled.IsChecked.Value;
            generalSettings.IsUriAssociationCheckEnabled = CheckBoxIsUriAssociationCheckEnabled.IsChecked.Value;
            generalSettings.IsSafeShutdownEnabled = CheckBoxIsSafeShutdownEnabled.IsChecked.Value;

            var isStartableOnSystemLogin = CheckBoxIsStartableOnSystemLogin.IsChecked.Value;
            if (isStartableOnSystemLogin != generalSettings.IsStartableOnSystemLogin) {
                generalSettings.IsStartableOnSystemLogin = isStartableOnSystemLogin;

                if (isStartableOnSystemLogin) {
                    var shell = new WshShell();
                    var shortcut = (WshShortcut)shell.CreateShortcut(StaticObjects.ApplicationStartupShortcutPath);

                    // Start the application hidden (on the tray)
                    shortcut.TargetPath = StaticObjects.ApplicationPath;
                    shortcut.Arguments = "-hidewindow";
                    shortcut.IconLocation = StaticObjects.ApplicationPath;
                    shortcut.WorkingDirectory = StaticObjects.ApplicationBaseDirectory;

                    shortcut.Save();

                } else {
                    if (File.Exists(StaticObjects.ApplicationStartupShortcutPath)) {
                        File.Delete(StaticObjects.ApplicationStartupShortcutPath);
                    }
                }
            }
        }
    }
}
