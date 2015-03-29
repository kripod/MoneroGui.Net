using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class AddressBookEditDialog : Dialog<bool>
    {
        private int _editIndex = -1;

        private TextBox TextBoxLabel { get; set; }
        private TextBox TextBoxAddress { get; set; }
        private Button ButtonOk { get; set; }

        private IList<SettingsManager.ConfigElementContact> CurrentContacts { get; set; }

        public int OverwriteIndex { get; private set; }

        private int EditIndex {
            get { return _editIndex; }
            set { _editIndex = value; }
        }

        public string Label {
            get { return TextBoxLabel.Text; }
        }

        public string Address {
            get { return TextBoxAddress.Text; }
        }

        public AddressBookEditDialog(IList<SettingsManager.ConfigElementContact> currentContacts, int editIndex = -1)
        {
            string windowTitle;

            CurrentContacts = currentContacts;
            EditIndex = editIndex;

            if (EditIndex >= 0) {
                // Mode: Edit
                windowTitle = MoneroGUI.Desktop.Properties.Resources.AddressBookEditWindowTitleEdit;

                var editedContact = CurrentContacts[EditIndex];
                TextBoxLabel = new TextBox { Text = editedContact.Label, PlaceholderText = editedContact.Label };
                TextBoxAddress = new TextBox { Text = editedContact.Address, PlaceholderText = editedContact.Address };

                TextBoxAddress.SelectAll();
                TextBoxAddress.Focus();

            } else {
                // Mode: Add
                windowTitle = MoneroGUI.Desktop.Properties.Resources.AddressBookEditWindowTitleAdd;

                TextBoxLabel = new TextBox();
                TextBoxAddress = new TextBox();
            }

            this.SetWindowProperties(
                windowTitle,
                new Size(400, 0)
            );

            TextBoxLabel.TextChanged += OnTextBoxLabelTextChanged;
            TextBoxAddress.TextChanged += OnTextBoxAddressTextChanged;

            RenderContent();
        }

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            ButtonOk = new Button { Text = MoneroGUI.Desktop.Properties.Resources.TextOk };
            ButtonOk.Click += delegate {
                Close(true);
            };
            DefaultButton = ButtonOk;

            var buttonCancel = new Button { Text = MoneroGUI.Desktop.Properties.Resources.TextCancel };
            AbortButton = buttonCancel;
            AbortButton.Click += delegate { Close(false); };

            Content = new TableLayout(
                new TableLayout(
                    new TableRow(
                        new Label { Text = MoneroGUI.Desktop.Properties.Resources.TextLabel + MoneroGUI.Desktop.Properties.Resources.PunctuationColon },
                        TextBoxLabel
                    ),
                    new TableRow(
                        new Label { Text = MoneroGUI.Desktop.Properties.Resources.TextAddress + MoneroGUI.Desktop.Properties.Resources.PunctuationColon },
                        TextBoxAddress
                    )
                ) { Spacing = Utilities.Spacing3 },

                new TableLayout(
                    new TableRow(
                        new TableCell { ScaleWidth = true },
                        ButtonOk,
                        buttonCancel
                    )
                ) { Spacing = Utilities.Spacing3 }
            ) { Spacing = Utilities.Spacing3 };
        }

        void OnTextBoxLabelTextChanged(object sender, EventArgs e)
        {
            OverwriteIndex = CurrentContacts.IndexOfLabel(Label);

            if (OverwriteIndex >= 0 && OverwriteIndex != EditIndex) {
                // Notify the user of the overwriting operation
                TextBoxLabel.TextColor = Utilities.ColorForegroundWarning;

            } else {
                TextBoxLabel.TextColor = Utilities.ColorForegroundDefault;
            }

            CheckInputsValidity();
        }

        void OnTextBoxAddressTextChanged(object sender, EventArgs e)
        {
            CheckInputsValidity();
        }

        void CheckInputsValidity()
        {
            ButtonOk.Enabled = Label.Length > 0 && Address.Length > 0;
        }
    }
}
