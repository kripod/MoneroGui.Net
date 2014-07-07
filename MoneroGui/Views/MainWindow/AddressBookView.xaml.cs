using Jojatekok.MoneroGUI.Windows;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    public partial class AddressBookView : IExportable
    {
        private static readonly ObservableCollection<SettingsManager.ConfigElementContact> DataSourceAddressBook = StaticObjects.DataSourceAddressBook;

        public bool IsDialogModeEnabled {
            get { return ButtonOk.Visibility == Visibility.Visible; }

            set {
                if (value) {
                    ButtonExport.Visibility = Visibility.Collapsed;
                    ButtonOk.Visibility = Visibility.Visible;

                } else {
                    ButtonOk.Visibility = Visibility.Collapsed;
                    ButtonExport.Visibility = Visibility.Visible;
                }
            }
        }

        public AddressBookView()
        {
            InitializeComponent();

            DataGridAddressBook.SelectedIndex = -1;
            this.SetDefaultFocusedElement(DataGridAddressBook);
        }

        private void DataGridAddressBook_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (IsInitialized) {
                var isButtonsEnabled = DataGridAddressBook.SelectedIndex >= 0;
                ButtonCopyAddress.IsEnabled = isButtonsEnabled;
                ButtonEdit.IsEnabled = isButtonsEnabled;
                ButtonDelete.IsEnabled = isButtonsEnabled;
                ButtonQrCode.IsEnabled = isButtonsEnabled;
                ButtonOk.IsEnabled = isButtonsEnabled;
            }
        }

        private void DataGridAddressBookCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check only for double clicks with the left mouse button
            if (e.ChangedButton != MouseButton.Left) return;

            if (IsDialogModeEnabled) {
                SetDialogResult();
            } else {
                EditSelectedContact();
            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddressBookEditWindow(Window.GetWindow(Parent), DataSourceAddressBook);
            if (dialog.ShowDialog() == true) {
                var overwriteIndex = dialog.OverwriteIndex;

                if (overwriteIndex < 0) {
                    // Add new item
                    DataSourceAddressBook.Add(new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address));
                    overwriteIndex = DataSourceAddressBook.Count - 1;

                } else {
                    // Overwrite existing item
                    DataSourceAddressBook[overwriteIndex] = new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address);
                }

                DataGridAddressBook.SelectedItem = DataSourceAddressBook[overwriteIndex];
            }

            this.SetFocusedElement(DataGridAddressBook);
        }

        private void ButtonCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null, "DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null");
            Clipboard.SetText((DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact).Address);

            this.SetFocusedElement(DataGridAddressBook);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            EditSelectedContact();
        }

        private void EditSelectedContact()
        {
            var editIndex = DataSourceAddressBook.IndexOf(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact);
            var dialog = new AddressBookEditWindow(Window.GetWindow(Parent), DataSourceAddressBook, editIndex);
            if (dialog.ShowDialog() == true) {
                var overwriteIndex = dialog.OverwriteIndex;
                if (overwriteIndex >= 0 && overwriteIndex != editIndex) {
                    DataSourceAddressBook.RemoveAt(dialog.OverwriteIndex);
                    if (overwriteIndex < editIndex) editIndex -= 1;
                }

                DataSourceAddressBook[editIndex] = new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address);
            }

            this.SetFocusedElement(DataGridAddressBook);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            DataSourceAddressBook.Remove(DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact);

            this.SetFocusedElement(DataGridAddressBook);
        }

        private void ButtonQrCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new QrCodeWindow(Window.GetWindow(Parent), DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact);
            dialog.ShowDialog();

            this.SetFocusedElement(DataGridAddressBook);
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Export();

            this.SetFocusedElement(DataGridAddressBook);
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult();
        }

        private void SetDialogResult()
        {
            var window = Window.GetWindow(Parent);
            if (window != null) window.DialogResult = true;
        }

        public void Export()
        {
            var dialog = new SaveFileDialog {
                Filter = Properties.Resources.TextFilterCsvFiles + "|" + Properties.Resources.TextFilterAllFiles,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

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
