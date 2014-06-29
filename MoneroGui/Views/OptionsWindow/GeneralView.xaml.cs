using IWshRuntimeLibrary;
using System.Diagnostics;
using File = System.IO.File;

namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class GeneralView
    {
        public GeneralView()
        {
            InitializeComponent();

#if DEBUG
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
#endif

            // Load settings
            var generalSettings = SettingsManager.General;
            CheckBoxStartableOnSystemLogin.IsChecked = generalSettings.IsStartableOnSystemLogin;
            CheckBoxSafeShutdownEnabled.IsChecked = generalSettings.IsSafeShutdownEnabled;
        }

        public void ApplySettings()
        {
            Debug.Assert(CheckBoxStartableOnSystemLogin.IsChecked != null, "CheckBoxStartableOnSystemLogin.IsChecked != null");
            Debug.Assert(CheckBoxSafeShutdownEnabled.IsChecked != null, "CheckBoxSafeShutdownEnabled.IsChecked != null");

            var generalSettings = SettingsManager.General;
            var isStartableOnSystemLogin = CheckBoxStartableOnSystemLogin.IsChecked.Value;
            var isSafeShutdownEnabled = CheckBoxSafeShutdownEnabled.IsChecked.Value;
            generalSettings.IsSafeShutdownEnabled = isSafeShutdownEnabled;

            if (isStartableOnSystemLogin != generalSettings.IsStartableOnSystemLogin) {
                generalSettings.IsStartableOnSystemLogin = isStartableOnSystemLogin;

                if (isStartableOnSystemLogin) {
                    var shell = new WshShell();
                    var shortcut = (WshShortcut)shell.CreateShortcut(StaticObjects.ApplicationStartupShortcutPath);

                    // TODO: Implement hidden mode
                    shortcut.TargetPath = StaticObjects.ApplicationPath;
                    shortcut.IconLocation = StaticObjects.ApplicationPath;
                    shortcut.WorkingDirectory = StaticObjects.ApplicationBaseDirectory;

                    shortcut.Save();

                } else {
                    if (File.Exists(StaticObjects.ApplicationStartupShortcutPath)) {
                        File.Delete(StaticObjects.ApplicationStartupShortcutPath);
                    }
                }
            }

            // TODO: Allow unsafe shutdown
        }
    }
}
