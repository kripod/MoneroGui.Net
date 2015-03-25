using Eto;
using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public class AddressBookView : TableLayout, IExportable
    {
        private static readonly FilterCollection<SettingsManager.ConfigElementContact> DataSourceAddressBook = Utilities.DataSourceAddressBook;

        private GridView GridViewAddressBook { get; set; }

        private Button ButtonCopyAddress { get; set; }
        private Button ButtonEdit { get; set; }
        private Button ButtonDelete { get; set; }
        private Button ButtonShowQrCode { get; set; }
        private Button ButtonExport { get; set; }
        private Button ButtonOk { get; set; }

        private Panel PanelButtonOk { get; set; }

        public bool IsDialogModeEnabled {
            get { return PanelButtonOk.Visible; }
            set { PanelButtonOk.Visible = value; }
        }

        public SettingsManager.ConfigElementContact SelectedContact {
            get { return GridViewAddressBook.SelectedItem as SettingsManager.ConfigElementContact; }
        }

        public AddressBookView()
        {
            Spacing = Utilities.Spacing3;

            GridViewAddressBook = Utilities.CreateGridView(
                Utilities.DataSourceAddressBook,
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Property<SettingsManager.ConfigElementContact, string>(o => o.Label) },
                    HeaderText = "Label" // TODO: Localize
                },
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Property<SettingsManager.ConfigElementContact, string>(o => o.Address) },
                    HeaderText = "Address" // TODO: Localize
                }
            );
            GridViewAddressBook.SelectedRowsChanged += OnGridViewAddressBookSelectedRowsChanged;
            GridViewAddressBook.CellDoubleClick += OnGridViewAddressBookCellDoubleClick;

            ButtonCopyAddress = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.AddressBookCopyAddress, null, Utilities.LoadImage("Copy"), OnButtonCopyAddressClick);
            ButtonEdit = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextEdit, null, Utilities.LoadImage("Edit"), OnButtonEditClick);
            ButtonDelete = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextDelete, null, Utilities.LoadImage("Delete"), OnButtonDeleteClick);
            ButtonShowQrCode = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextQrCode, null, Utilities.LoadImage("QrCode"), OnButtonShowQrCodeClick);
            ButtonExport = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextExport, null, Utilities.LoadImage("Export"), OnButtonExportClick);
            ButtonOk = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextOk, null, Utilities.LoadImage("Ok"), SetDialogResult);

            ButtonCopyAddress.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonShowQrCode.Enabled = false;
            ButtonOk.Enabled = false;

            PanelButtonOk = new Panel {
                Content = ButtonOk,
                Padding = new Padding(Utilities.Padding3, 0, 0, 0),
                Visible = false
            };

            Rows.Add(
                new TableRow(GridViewAddressBook) { ScaleHeight = true }
            );

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        Utilities.CreateButton(() =>
                            MoneroGUI.Desktop.Properties.Resources.TextNew,
                            null,
                            Utilities.LoadImage("Add"),
                            OnButtonNewClick
                        ),
                        ButtonCopyAddress,
                        ButtonEdit,
                        ButtonDelete,

                        new TableCell { ScaleWidth = true },

                        ButtonShowQrCode,
                        new TableRow(
                            ButtonExport,
                            PanelButtonOk
                        )
                    )
                ) { Spacing = Utilities.Spacing3 }
            );
        }

        void OnGridViewAddressBookSelectedRowsChanged(object sender, EventArgs e)
        {
            var isButtonsEnabled = GridViewAddressBook.SelectedItem != null;
            ButtonCopyAddress.Enabled = isButtonsEnabled;
            ButtonEdit.Enabled = isButtonsEnabled;
            ButtonDelete.Enabled = isButtonsEnabled;
            ButtonShowQrCode.Enabled = isButtonsEnabled;
            ButtonOk.Enabled = isButtonsEnabled;
        }

        void OnGridViewAddressBookCellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            if (IsDialogModeEnabled) {
                SetDialogResult();
            } else {
                ShowSelectedContactQrCode();
            }
        }

        private void OnButtonNewClick()
        {
            var dialog = new AddressBookEditDialog(DataSourceAddressBook);
            if (dialog.ShowModal(this)) {
                var overwriteIndex = dialog.OverwriteIndex;

                if (overwriteIndex < 0) {
                    // Add new item
                    DataSourceAddressBook.Add(new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address));
                    overwriteIndex = DataSourceAddressBook.Count - 1;

                } else {
                    // Overwrite existing item
                    DataSourceAddressBook.RemoveAt(overwriteIndex);
                    DataSourceAddressBook.Insert(overwriteIndex, new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address));
                }

                GridViewAddressBook.SelectRow(overwriteIndex);
            }

            GridViewAddressBook.Focus();
        }

        private void OnButtonCopyAddressClick()
        {
            Utilities.Clipboard.Text = SelectedContact.Address;

            GridViewAddressBook.Focus();
        }

        private void OnButtonEditClick()
        {
            EditSelectedContact();
        }

        private void OnButtonDeleteClick()
        {
            DataSourceAddressBook.Remove(SelectedContact);

            GridViewAddressBook.Focus();
        }

        private void OnButtonShowQrCodeClick()
        {
            ShowSelectedContactQrCode();
        }

        private void OnButtonExportClick()
        {
            Export();

            GridViewAddressBook.Focus();
        }

        private void EditSelectedContact()
        {
            var editIndex = DataSourceAddressBook.IndexOf(SelectedContact);

            using (var dialog = new AddressBookEditDialog(DataSourceAddressBook, editIndex)) {
                if (dialog.ShowModal(this)) {
                    var overwriteIndex = dialog.OverwriteIndex;
                    if (overwriteIndex >= 0 && overwriteIndex != editIndex) {
                        DataSourceAddressBook.RemoveAt(dialog.OverwriteIndex);
                        if (overwriteIndex < editIndex) editIndex -= 1;
                    }

                    DataSourceAddressBook.RemoveAt(editIndex);
                    DataSourceAddressBook.Insert(editIndex, new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address));
                }
            }

            GridViewAddressBook.Focus();
        }

        private void ShowSelectedContactQrCode()
        {
            using (var dialog = new QrCodeDialog(SelectedContact)) {
                dialog.ShowModal(this);
            }

            GridViewAddressBook.Focus();
        }

        private void SetDialogResult()
        {
            var dialog = Parent as Dialog<SettingsManager.ConfigElementContact>;
            if (dialog != null) dialog.Close(SelectedContact);
        }

        public void Export()
        {
            using (
                var dialog = new SaveFileDialog {
                    Filters = new HashSet<FileDialogFilter> {
                        new FileDialogFilter(MoneroGUI.Desktop.Properties.Resources.TextFilterCsvFiles, Utilities.FileFilterCsv),
                        new FileDialogFilter(MoneroGUI.Desktop.Properties.Resources.TextFilterAllFiles, Utilities.FileFilterAll)
                    },
                    Directory = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                }
            ) {
                if (dialog.ShowDialog(this) != DialogResult.Ok) return;

                Export(dialog.FileName);
            }
        }

        public void Export(string fileName)
        {
            var dataTable = new DataTable();

            var columns = GridViewAddressBook.Columns;
            for (var i = 0; i < columns.Count; i++) {
                dataTable.ColumnHeaders.Add(columns[i].HeaderText);
            }

            var dataSource = Utilities.DataSourceAddressBook;
            for (var i = 0; i < dataSource.Items.Count; i++) {
                var contact = dataSource.Items[i];
                dataTable.Rows.Add(
                    new List<object> {
                        contact.Label,
                        contact.Address
                    }
                );
            }
            
            dataTable.ExportToCsvAsync(fileName);
        }
    }
}
