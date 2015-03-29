using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Windows;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Desktop.Controls
{
    public sealed class CoinSender : TableLayout, ICloneable, INotifyPropertyChanged
    {
        private readonly TextBox _textBoxAddress = Utilities.CreateTextBox(
            null,
            () => MoneroGUI.Desktop.Properties.Resources.TextHintAddress,
            new Font(FontFamilies.Monospace, Utilities.FontSize1)
        );
        private readonly TextBox _textBoxLabel = Utilities.CreateTextBox(
            null,
            () => MoneroGUI.Desktop.Properties.Resources.TextHintLabel
        );
        private double _amount;

        public string Address {
            get { return TextBoxAddress.Text; }
            set { TextBoxAddress.Text = value; }
        }

        public string Label {
            get { return TextBoxLabel.Text; }
            set { TextBoxLabel.Text = value; }
        }

        public double Amount {
            get { return _amount; }
            set {
                _amount = value;
                OnPropertyChanged();
            }
        }

        private TextBox TextBoxAddress { get { return _textBoxAddress; } }
        private TextBox TextBoxLabel { get { return _textBoxLabel; } }

        public CoinSender()
        {
            Padding = new Padding(0, Utilities.Padding5, 0, 0);
            Spacing = Utilities.Spacing2;

            var buttonShowAddressBook = new Button { Image = Utilities.LoadImage("Contact") };
            buttonShowAddressBook.Click += OnButtonShowAddressBookClick;

            var buttonPasteAddress = new Button { Image = Utilities.LoadImage("Paste") };
            buttonPasteAddress.Click += OnButtonPasteAddressClick;

            var buttonRemoveSelf = new Button { Image = Utilities.LoadImage("Delete") };
            buttonRemoveSelf.Click += OnButtonRemoveSelfClick;

            Rows.Add(new TableRow(
                Utilities.CreateLabel(() =>
                    MoneroGUI.Desktop.Properties.Resources.TextAddress +
                    MoneroGUI.Desktop.Properties.Resources.PunctuationColon
                ),

                new TableCell(TextBoxAddress, true),

                buttonShowAddressBook,
                buttonPasteAddress,
                buttonRemoveSelf
            ));

            Rows.Add(new TableRow(
                Utilities.CreateLabel(() =>
                    MoneroGUI.Desktop.Properties.Resources.TextLabel +
                    MoneroGUI.Desktop.Properties.Resources.PunctuationColon
                ),

                new TableCell(TextBoxLabel, true)
            ));

            Rows.Add(new TableRow(
                Utilities.CreateLabel(() =>
                    MoneroGUI.Desktop.Properties.Resources.TextAmount +
                    MoneroGUI.Desktop.Properties.Resources.PunctuationColon
                ),

                new TableCell(
                    Utilities.CreateNumericUpDown(this, o => o.Amount),
                    true
                )
            ));
        }

        void OnButtonShowAddressBookClick(object sender, EventArgs e)
        {
            using (var dialog = new AddressBookDialog()) {
                var contact = dialog.ShowModal(this);
                if (contact != null) {
                    Address = contact.Address;
                    Label = contact.Label;
                }
            }
        }

        void OnButtonPasteAddressClick(object sender, EventArgs e)
        {
            Address = Utilities.Clipboard.Text;
            TextBoxLabel.Focus();
        }

        void OnButtonRemoveSelfClick(object sender, EventArgs e)
        {
            var tableLayout = Parent as TableLayout;
            Debug.Assert(tableLayout != null, "tableLayout != null");

            var tableLayoutRows = tableLayout.Rows;
            if (tableLayoutRows.Count == 2) {
                Clear();
                TextBoxAddress.Focus();
                return;
            }

            for (var i = tableLayoutRows.Count - 2; i >= 0; i--) {
                if (tableLayoutRows[i].Cells[0].Control == this) {
                    tableLayout.Remove(this);
                    tableLayoutRows.RemoveAt(i);
                    return;
                }
            }
        }

        public void Clear()
        {
            Address = "";
            Label = "";
            Amount = 0;
        }

        public bool IsRecipientValid()
        {
            return MoneroAPI.Utilities.IsAddressValid(Address) && Amount > 0;
        }

        public object Clone()
        {
            return new CoinSender {
                Address = Address,
                Label = Label,
                Amount = Amount
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
