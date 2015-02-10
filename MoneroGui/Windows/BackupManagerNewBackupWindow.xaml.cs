using Jojatekok.MoneroAPI;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class BackupManagerNewBackupWindow
    {
        public enum AccountBackupOption
        {
            DefaultPath,
            CustomPath,
            DeterministicAccountSeed
        }

        public AccountBackupOption SelectedAccountBackupOption {
            get {
                if (RadioButtonDeterministicAccountSeed.IsChecked == true) return AccountBackupOption.DeterministicAccountSeed;
                if (RadioButtonDefaultPath.IsChecked == true) return AccountBackupOption.DefaultPath;
                return AccountBackupOption.CustomPath;
            }
        }

        public string BackupDirectory { get; private set; }

        private BackupManagerNewBackupWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();

            TextBlockDefaultBackupDirectory.Text = "(" + Path.GetFullPath(SettingsManager.Paths.DirectoryAccountBackups) + ")";
        }

        public BackupManagerNewBackupWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private async void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            switch (SelectedAccountBackupOption) {
                case AccountBackupOption.DeterministicAccountSeed:
                    var mnemonicKey = StaticObjects.MoneroRpcManager.AccountManager.QueryKey(AccountKeyType.Mnemonic);
                    if (mnemonicKey == null) {
                        this.ShowError(Properties.Resources.BackupManagerNewBackupWindowErrorRetrievingSeed);
                        return;
                    }

                    var deterministicAccountSeedDialog = new BackupManagerDeterministicAccountSeedWindow(this, mnemonicKey);
                    deterministicAccountSeedDialog.ShowDialog();
                    DialogResult = false;
                    break;

                case AccountBackupOption.DefaultPath:
                    BackupDirectory = await StaticObjects.MoneroProcessManager.AccountManager.BackupAsync();
                    DialogResult = true;
                    break;

                case AccountBackupOption.CustomPath:
                    var pathDialog = new VistaFolderBrowserDialog { RootFolder = Environment.SpecialFolder.MyComputer };
                    if (pathDialog.ShowDialog() == true) {
                        BackupDirectory = await StaticObjects.MoneroProcessManager.AccountManager.BackupAsync(pathDialog.SelectedPath);
                        DialogResult = true;
                    }
                    break;
            }

            Close();
        }
    }
}
