using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class BackupManagerWindow : INotifyPropertyChanged
    {
        private bool _isRegularWalletBackupEnabled = SettingsManager.General.IsRegularWalletBackupEnabled;
        public bool IsRegularWalletBackupEnabled {
            get { return _isRegularWalletBackupEnabled; }

            set {
                _isRegularWalletBackupEnabled = value;
                OnPropertyChanged();

                SettingsManager.General.IsRegularWalletBackupEnabled = value;
            }
        }

        private string BaseBackupDirectory { get; set; }

        private BackupManagerWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();

            BaseBackupDirectory = SettingsManager.Paths.DirectoryWalletBackups;
            Task.Factory.StartNew(LoadBackups);
        }

        public BackupManagerWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private void LoadBackups()
        {
            if (!Directory.Exists(BaseBackupDirectory)) return;

            var backups = Directory.GetDirectories(BaseBackupDirectory, "*", SearchOption.TopDirectoryOnly);
            var backupDates = new List<string>(backups.Length);

            for (var i = backups.Length - 1; i >= 0; i--) {
                var backup = backups[i];
                var lastSlashIndex = backup.LastIndexOf('\\');

                if (lastSlashIndex >= 0) {
                    backup = backup.Substring(lastSlashIndex + 1);
                }

                backupDates.Add(backup);
            }

            backupDates.Sort();
            Dispatcher.BeginInvoke(new Action(() => {
                for (var i = backupDates.Count - 1; i >= 0; i--) {
                    ListBoxBackups.Items.Add(backupDates[i]);
                }
            }), DispatcherPriority.DataBind);
        }

        private void ListBoxBackups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonRestoreBackupFromSelection.IsEnabled = ListBoxBackups.SelectedIndex >= 0;
        }

        private void ButtonBrowseBackupsInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(BaseBackupDirectory)) {
                Process.Start(BaseBackupDirectory);
            }

            this.SetFocusedElement(ListBoxBackups);
        }

        private async void ButtonNewBackup_Click(object sender, RoutedEventArgs e)
        {
            this.ShowWarning(Properties.Resources.BackupManagerWindowWarningNewBackup);

            var dialog = new BackupManagerNewBackupWindow(this);
            if (dialog.ShowDialog() == true) {
                string backupName;

                if (dialog.IsDefaultLocationSelected) {
                    backupName = dialog.BackupDirectory.Substring(BaseBackupDirectory.Length);
                } else {
                    backupName = await Task.Factory.StartNew(() => IsDirectoryAvailableInBackups(dialog.BackupDirectory));
                    if (backupName == null) {
                        this.SetFocusedElement(ListBoxBackups);
                        return;
                    }
                }

                var listViewItems = ListBoxBackups.Items;
                if (!listViewItems.Contains(backupName)) listViewItems.Insert(0, backupName);
            }

            this.SetFocusedElement(ListBoxBackups);
        }

        private async void ButtonRestoreBackupFromSelection_Click(object sender, RoutedEventArgs e)
        {
            var selectedBackup = ListBoxBackups.SelectedItem as string;
            if (selectedBackup != null) {
                await TryRestoreWalletFromDirectoryAsync(Path.Combine(BaseBackupDirectory, selectedBackup));
            }

            this.SetFocusedElement(ListBoxBackups);
        }

        private async void ButtonRestoreBackupFromDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog { RootFolder = Environment.SpecialFolder.MyComputer };
            if (dialog.ShowDialog() == true) {
                await TryRestoreWalletFromDirectoryAsync(dialog.SelectedPath);
            }

            this.SetFocusedElement(ListBoxBackups);
        }

        private async Task TryRestoreWalletFromDirectoryAsync(string directoryToRestore)
        {
            var isRestoreSuccessful = await Task.Factory.StartNew(() => RestoreWalletFromDirectory(directoryToRestore));

            if (isRestoreSuccessful) {
                RestoreShowSuccess();
            } else {
                RestoreShowFailure(directoryToRestore);
            }
        }

        private static bool RestoreWalletFromDirectory(string directoryToRestore)
        {
            // Get the list of files to restore
            if (!Directory.Exists(directoryToRestore)) return false;
            var files = Directory.GetFiles(directoryToRestore, "*", SearchOption.TopDirectoryOnly);
            if (files.Length == 0) return false;

            // Initialize variables
            var fileWalletData = SettingsManager.Paths.FileWalletData;
            var directoryWalletData = Helper.GetDirectoryOfFile(fileWalletData);
            var baseWalletFileName = Helper.GetFileNameWithoutExtension(fileWalletData);

            // Stop the wallet
            StaticObjects.MoneroClient.Wallet.Stop();

            // Restore the wanted files (with name conversion)
            for (var i = files.Length - 1; i >= 0; i--) {
                var sourceFile = files[i];
                var destinationFile = baseWalletFileName + Helper.GetFileExtension(sourceFile);
                File.Copy(sourceFile, Path.Combine(directoryWalletData, destinationFile), true);
            }

            // Restart the wallet
            StaticObjects.MoneroClient.Wallet.Start();
            return true;
        }

        private void RestoreShowSuccess()
        {
            this.ShowInformation(Properties.Resources.BackupManagerWindowInformationRestoreSuccessful);
            Close();
        }

        private void RestoreShowFailure(string path)
        {
            this.ShowError(string.Format(Helper.InvariantCulture, Properties.Resources.BackupManagerWindowErrorRestore, path));
        }

        private string IsDirectoryAvailableInBackups(string directory)
        {
            var lastSlashIndex = directory.LastIndexOf('\\');
            if (lastSlashIndex >= 0) {
                directory = directory.Substring(lastSlashIndex + 1);
            }

            if (!Directory.Exists(BaseBackupDirectory)) return null;
            if (Directory.GetDirectories(BaseBackupDirectory, directory, SearchOption.TopDirectoryOnly).Length == 0) return null;

            return directory;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
