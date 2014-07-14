using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class OptionsWindow
    {
        private OptionsWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;
            SourceInitialized += delegate { this.SetWindowButtons(false, true); };

            InitializeComponent();
        }

        public OptionsWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.IsAutoSaveEnabled = false;

            var items = TabControl.Items;
            for (var i = items.Count - 1; i >= 0; i--) {
                var tabItem = items[i] as TabItem;
                Debug.Assert(tabItem != null, "tabItem != null");
                var content = tabItem.Content as ISettingsView;
                Debug.Assert(content != null, "content != null");
                content.ApplySettings();
            }

            SettingsManager.IsAutoSaveEnabled = true;
            SettingsManager.SaveSettings();
            Close();
        }
    }
}
