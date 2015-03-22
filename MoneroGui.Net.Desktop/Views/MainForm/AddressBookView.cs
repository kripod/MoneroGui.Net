using Eto;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Windows;
using System;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public class AddressBookView : TableLayout
    {
        private readonly Button _buttonCopyAddress = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.AddressBookCopyAddress, null, Utilities.LoadImage("Copy"));
        private readonly Button _buttonEdit = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextEdit, null, Utilities.LoadImage("Edit"));
        private readonly Button _buttonDelete = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextDelete, null, Utilities.LoadImage("Delete"));
        private readonly Button _buttonShowQrCode = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextQrCode, null, Utilities.LoadImage("QrCode"));
        private readonly Button _buttonExport = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextExport, null, Utilities.LoadImage("Export"));
        private readonly Button _buttonOk = Utilities.CreateButton(() => MoneroGUI.Desktop.Properties.Resources.TextOk, null);

        private GridView GridViewAddressBook { get; set; }

        private Button ButtonCopyAddress { get { return _buttonCopyAddress; } }
        private Button ButtonEdit { get { return _buttonEdit; } }
        private Button ButtonDelete { get { return _buttonDelete; } }
        private Button ButtonShowQrCode { get { return _buttonShowQrCode; } }
        private Button ButtonExport { get { return _buttonExport; } }
        private Button ButtonOk { get { return _buttonOk; } }

        private static readonly FilterCollection<SettingsManager.ConfigElementContact> DataSourceAddressBook = Utilities.DataSourceAddressBook;

        public bool IsDialogModeEnabled {
            get { return ButtonOk.Visible; }
            set { ButtonOk.Visible = value; }
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

            ButtonCopyAddress.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonShowQrCode.Enabled = false;
            ButtonExport.Enabled = false;
            ButtonOk.Enabled = false;

            ButtonCopyAddress.Click += delegate { OnButtonCopyAddressClick(); };
            ButtonEdit.Click += delegate { OnButtonEditClick(); };
            ButtonDelete.Click += delegate { OnButtonDeleteClick(); };
            ButtonShowQrCode.Click += delegate { OnButtonShowQrCodeClick(); };
            ButtonExport.Click += delegate { OnButtonExportClick(); };
            ButtonOk.Click += delegate { OnButtonOkClick(); };

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
                        ButtonExport,
                        ButtonOk
                    )
                ) { Spacing = Utilities.Spacing3 }
            );
        }

        void OnGridViewAddressBookSelectedRowsChanged(object sender, EventArgs e)
        {
            if (Loaded) {
                var isButtonsEnabled = GridViewAddressBook.SelectedItem != null;
                ButtonCopyAddress.Enabled = isButtonsEnabled;
                ButtonEdit.Enabled = isButtonsEnabled;
                ButtonDelete.Enabled = isButtonsEnabled;
                ButtonShowQrCode.Enabled = isButtonsEnabled;
                ButtonOk.Enabled = isButtonsEnabled;
            }
        }

        void OnGridViewAddressBookCellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            if (IsDialogModeEnabled) {
                SetDialogResult();
            } else {
                ShowSelectedContactQrCode();
            }
        }

        private void EditSelectedContact()
        {
            var editIndex = DataSourceAddressBook.IndexOf(GridViewAddressBook.SelectedItem as SettingsManager.ConfigElementContact);

            using (var dialog = new AddressBookEditDialog(DataSourceAddressBook, editIndex)) {
                if (dialog.ShowModal(this)) {
                    var overwriteIndex = dialog.OverwriteIndex;
                    if (overwriteIndex >= 0 && overwriteIndex != editIndex) {
                        DataSourceAddressBook.RemoveAt(dialog.OverwriteIndex);
                        if (overwriteIndex < editIndex) editIndex -= 1;
                    }

                    DataSourceAddressBook[editIndex] = new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address);
                }
            }

            GridViewAddressBook.Focus();
        }

        private void ShowSelectedContactQrCode()
        {
            using (var dialog = new QrCodeDialog(GridViewAddressBook.SelectedItem as SettingsManager.ConfigElementContact)) {
                dialog.ShowModal(this);
            }

            GridViewAddressBook.Focus();
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
                    DataSourceAddressBook[overwriteIndex] = new SettingsManager.ConfigElementContact(dialog.Label, dialog.Address);
                }

                GridViewAddressBook.SelectRow(overwriteIndex);
            }

            GridViewAddressBook.Focus();
        }

        private void OnButtonCopyAddressClick()
        {
            Debug.Assert(GridViewAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null, "GridViewAddressBook.SelectedItem as SettingsManager.ConfigElementContact != null");
            Utilities.Clipboard.Text = (GridViewAddressBook.SelectedItem as SettingsManager.ConfigElementContact).Address;

            GridViewAddressBook.Focus();
        }

        private void OnButtonEditClick()
        {
            EditSelectedContact();
        }

        private void OnButtonDeleteClick()
        {
            DataSourceAddressBook.Remove(GridViewAddressBook.SelectedItem as SettingsManager.ConfigElementContact);

            GridViewAddressBook.Focus();
        }

        private void OnButtonShowQrCodeClick()
        {
            ShowSelectedContactQrCode();
        }

        private void OnButtonExportClick()
        {
            //Export();

            GridViewAddressBook.Focus();
        }

        private void OnButtonOkClick()
        {
            SetDialogResult();
        }

        private void SetDialogResult()
        {
            var dialog = Parent as Dialog<bool>;
            if (dialog != null) dialog.Close(true);
        }

        /*public void Export()
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
                    dataTable.Rows.Add(
                        "=\"" + contact.Label + "\"",
                        "=\"" + contact.Address + "\""
                    );
                }

                dataTable.ExportToCsvAsync(fileName);
            }
        }*/
    }
}
