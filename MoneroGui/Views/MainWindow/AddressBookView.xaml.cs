using Jojatekok.MoneroGUI.Windows;
using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    public partial class AddressBookView : IExportable
    {
        public AddressBookView()
        {
            InitializeComponent();

            DataGridAddressBook.SelectedIndex = -1;
        }

        private void DataGridAddressBook_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (IsInitialized) {
                var isButtonsEnabled = DataGridAddressBook.SelectedIndex >= 0;
                ButtonCopyAddress.IsEnabled = isButtonsEnabled;
                ButtonEdit.IsEnabled = isButtonsEnabled;
                ButtonDelete.IsEnabled = isButtonsEnabled;
                ButtonQrCode.IsEnabled = isButtonsEnabled;
            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddressBookEditWindow(Window.GetWindow(Parent), ViewModel.DataSource);
            if (dialog.ShowDialog() == true) {
                var overwriteIndex = dialog.OverwriteIndex;

                if (overwriteIndex < 0) {
                    // Add new item
                    ViewModel.DataSource.Add(new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address));
                    overwriteIndex = ViewModel.DataSource.Count - 1;

                } else {
                    // Overwrite existing item
                    ViewModel.DataSource[overwriteIndex] = new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address);
                }

                DataGridAddressBook.SelectedItem = ViewModel.DataSource[overwriteIndex];
            }

            DataGridAddressBook.Focus();
        }

        private void ButtonCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null, "DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null");
            Clipboard.SetText((DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact).Address);

            DataGridAddressBook.Focus();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var editIndex = ViewModel.DataSource.IndexOf(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact);
            var dialog = new AddressBookEditWindow(Window.GetWindow(Parent), ViewModel.DataSource, editIndex);
            if (dialog.ShowDialog() == true) {
                var overwriteIndex = dialog.OverwriteIndex;
                if (overwriteIndex >= 0 && overwriteIndex != editIndex) {
                    ViewModel.DataSource.RemoveAt(dialog.OverwriteIndex);
                    if (overwriteIndex < editIndex) editIndex -= 1;
                }

                ViewModel.DataSource[editIndex] = new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address);
            }

            DataGridAddressBook.Focus();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DataSource.Remove(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact);

            DataGridAddressBook.Focus();
        }

        private void ButtonQrCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new QrCodeWindow(Window.GetWindow(Parent), DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact);
            dialog.ShowDialog();

            DataGridAddressBook.Focus();
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Export();

            DataGridAddressBook.Focus();
        }

        public void Export()
        {
            var dialog = new SaveFileDialog { Filter = Properties.Resources.TextFilterCsvFiles + "|" + Properties.Resources.TextFilterAllFiles,
                                              InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
            if (dialog.ShowDialog() == true) Export(dialog.FileName);
        }

        public void Export(string fileName)
        {
            using (var dataTable = new DataTable { Locale = Helper.InvariantCulture }) {
                var columns = DataGridAddressBook.Columns;
                for (var i = 0; i < columns.Count; i++) {
                    dataTable.Columns.Add(columns[i].Header.ToString());
                }

                for (var i = 0; i < DataGridAddressBook.Items.Count; i++) {
                    var contact = DataGridAddressBook.Items[i] as SettingsManager.ConfigElementContact;
                    Debug.Assert(contact != null, "contact != null");
                    dataTable.Rows.Add("=\"" + contact.Label + "\"",
                                       "=\"" + contact.Address + "\"");
                }

                dataTable.ExportToCsvAsync(fileName);
            }
        }
    }
}
