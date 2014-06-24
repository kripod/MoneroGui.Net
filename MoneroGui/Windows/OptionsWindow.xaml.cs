using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class OptionsWindow
    {
        private OptionsWindow()
        {
            Icon = StaticObjects.ApplicationIcon;
            Loaded += delegate { this.SetWindowButtons(false, true); };

            InitializeComponent();
        }

        public OptionsWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.IsAutoSaveEnabled = false;

            GeneralView.ApplySettings();
            PathsView.ApplySettings();
            AppearanceView.ApplySettings();

            SettingsManager.IsAutoSaveEnabled = true;
            SettingsManager.SaveSettings();
            Close();
        }
    }
}
