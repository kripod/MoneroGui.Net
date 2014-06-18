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
                ButtonDelete.IsEnabled = isButtonsEnabled;
            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddressBookAddWindow(Window.GetWindow(Parent));
            if (dialog.ShowDialog() == true) {
                ViewModel.DataSource.Add(new SettingsManager.ConfigElementContact(dialog.TextBoxLabel.Text, dialog.TextBoxAddress.Text));
            }
        }

        private void ButtonCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null, "DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null");
            Clipboard.SetText((DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact).Address);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DataSource.Remove(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact);
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Export();
        }

        public void Export()
        {
            var dialog = new SaveFileDialog { Filter = Properties.Resources.TextFilterCsvFiles + "|" + Properties.Resources.TextFilterAllFiles,
                                              InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
            if (dialog.ShowDialog() == true) Export(dialog.FileName);
        }

        public void Export(string fileName)
        {
            using (var dataTable = new DataTable()) {
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
