using Ookii.Dialogs.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class BackupManagerNewBackupWindow
    {
        public bool IsDefaultLocationSelected {
            get {
                Debug.Assert(RadioButtonDefaultLocation.IsChecked != null, "RadioButtonDefaultLocation.IsChecked != null");
                return RadioButtonDefaultLocation.IsChecked.Value;
            }
        }

        public string BackupDirectory { get; private set; }

        private BackupManagerNewBackupWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();

            TextBlockDefaultBackupDirectory.Text = "(" + Path.GetFullPath(SettingsManager.Paths.DirectoryWalletBackups) + ")";
        }

        public BackupManagerNewBackupWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private async void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (IsDefaultLocationSelected) {
                BackupDirectory = await StaticObjects.MoneroClient.Wallet.BackupAsync();
                DialogResult = true;

            } else {
                var dialog = new VistaFolderBrowserDialog { RootFolder = Environment.SpecialFolder.MyComputer };

                if (dialog.ShowDialog() == true) {
                    BackupDirectory = await StaticObjects.MoneroClient.Wallet.BackupAsync(dialog.SelectedPath);
                    DialogResult = true;
                }
            }

            Close();
        }
    }
}
