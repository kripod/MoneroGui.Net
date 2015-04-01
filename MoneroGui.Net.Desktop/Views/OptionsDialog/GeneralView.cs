using Eto.Drawing;
using Eto.Forms;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Desktop.Views.OptionsDialog
{
    public class GeneralView : TableLayout, IOptionsTabPageView
    {
        private readonly CheckBox _checkBoxIsUpdateCheckEnabled = new CheckBox { Text = Desktop.Properties.Resources.OptionsGeneralIsUpdateCheckEnabled };
        private readonly CheckBox _checkBoxIsUpdateCheckForTestBuildsEnabled = new CheckBox { Text = Desktop.Properties.Resources.OptionsGeneralIsUpdateCheckForTestBuildsEnabled };
        private readonly CheckBox _checkBoxIsStartableOnSystemLogin = new CheckBox { Text = Desktop.Properties.Resources.OptionsGeneralIsStartableOnSystemLogin };
        private readonly CheckBox _checkBoxIsUriAssociationCheckEnabled = new CheckBox { Text = Desktop.Properties.Resources.OptionsGeneralIsUriAssociationCheckEnabled };
        private readonly CheckBox _checkBoxIsSafeShutdownEnabled = new CheckBox { Text = Desktop.Properties.Resources.OptionsGeneralIsSafeShutdownEnabled };

        private CheckBox CheckBoxIsUpdateCheckEnabled {
            get { return _checkBoxIsUpdateCheckEnabled; }
        }

        private CheckBox CheckBoxIsUpdateCheckForTestBuildsEnabled {
            get { return _checkBoxIsUpdateCheckForTestBuildsEnabled; }
        }

        private CheckBox CheckBoxIsStartableOnSystemLogin {
            get { return _checkBoxIsStartableOnSystemLogin; }
        }

        private CheckBox CheckBoxIsUriAssociationCheckEnabled {
            get { return _checkBoxIsUriAssociationCheckEnabled; }
        }

        private CheckBox CheckBoxIsSafeShutdownEnabled {
            get { return _checkBoxIsSafeShutdownEnabled; }
        }

        public GeneralView()
        {
            Spacing = Utilities.Spacing2;

            LoadSettings();
            CheckBoxIsUpdateCheckForTestBuildsEnabled.Enabled = CheckBoxIsUpdateCheckEnabled.Checked == true;

            // TODO: Enable the option below
            CheckBoxIsStartableOnSystemLogin.Enabled = false;

            CheckBoxIsUpdateCheckEnabled.CheckedChanged += delegate {
                CheckBoxIsUpdateCheckForTestBuildsEnabled.Enabled = CheckBoxIsUpdateCheckEnabled.Checked == true;
            };

            Rows.Add(CheckBoxIsUpdateCheckEnabled);

            Rows.Add(new Panel {
                Content = CheckBoxIsUpdateCheckForTestBuildsEnabled,
                Padding = new Padding(Utilities.Padding6, 0, 0, 0)
            });

            Rows.Add(new Panel {
                Content = CheckBoxIsStartableOnSystemLogin,
                Padding = new Padding(0, Utilities.Padding1, 0, 0)
            });

            Rows.Add(new Panel {
                Content = CheckBoxIsUriAssociationCheckEnabled,
                Padding = new Padding(0, Utilities.Padding1, 0, 0)
            });

            Rows.Add(new Panel {
                Content = CheckBoxIsSafeShutdownEnabled,
                Padding = new Padding(0, Utilities.Padding1, 0, 0)
            });

            Rows.Add(new TableRow());
        }

        void LoadSettings()
        {
            var generalSettings = SettingsManager.General;
            CheckBoxIsUpdateCheckEnabled.Checked = generalSettings.IsUpdateCheckEnabled;
            CheckBoxIsUpdateCheckForTestBuildsEnabled.Checked = generalSettings.IsUpdateCheckForTestBuildsEnabled;
            CheckBoxIsStartableOnSystemLogin.Checked = generalSettings.IsStartableOnSystemLogin;
            CheckBoxIsUriAssociationCheckEnabled.Checked = generalSettings.IsUriAssociationCheckEnabled;
            CheckBoxIsSafeShutdownEnabled.Checked = generalSettings.IsSafeShutdownEnabled;
        }

        public void ApplySettings()
        {
            Debug.Assert(CheckBoxIsUpdateCheckEnabled.Checked != null, "CheckBoxIsUpdateCheckEnabled.IsChecked != null");
            Debug.Assert(CheckBoxIsUpdateCheckForTestBuildsEnabled.Checked != null, "CheckBoxIsUpdateCheckForTestBuildsEnabled.IsChecked != null");
            Debug.Assert(CheckBoxIsUriAssociationCheckEnabled.Checked != null, "CheckBoxIsUriAssociationCheckEnabled.IsChecked != null");
            Debug.Assert(CheckBoxIsSafeShutdownEnabled.Checked != null, "CheckBoxSafeShutdownEnabled.IsChecked != null");

            var generalSettings = SettingsManager.General;
            generalSettings.IsUpdateCheckEnabled = CheckBoxIsUpdateCheckEnabled.Checked.Value;
            generalSettings.IsUpdateCheckForTestBuildsEnabled = CheckBoxIsUpdateCheckForTestBuildsEnabled.Checked.Value;
            generalSettings.IsUriAssociationCheckEnabled = CheckBoxIsUriAssociationCheckEnabled.Checked.Value;
            generalSettings.IsSafeShutdownEnabled = CheckBoxIsSafeShutdownEnabled.Checked.Value;
        }
    }
}
