using Eto.Drawing;
using Eto.Forms;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Desktop.Controls
{
    public sealed class CoinSender : TableLayout, ICloneable, INotifyPropertyChanged
    {
        private double _amount;

        private readonly TextBox _textBoxAddress = Utilities.CreateTextBox(
            null,
            () => MoneroGUI.Desktop.Properties.Resources.TextHintAddress,
            new Font(FontFamilies.Monospace, Utilities.FontSize1)
        );
        private readonly TextBox _textBoxLabel = Utilities.CreateTextBox(
            null,
            () => MoneroGUI.Desktop.Properties.Resources.TextHintLabel
        );

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

            Rows.Add(new TableRow(
                Utilities.CreateLabel(() =>
                    MoneroGUI.Desktop.Properties.Resources.TextAddress +
                    MoneroGUI.Desktop.Properties.Resources.PunctuationColon
                ),

                new TableCell(TextBoxAddress, true),

                new Button { Image = Utilities.LoadImage("Contact") },
                new Button { Image = Utilities.LoadImage("Paste") },
                new Button { Image = Utilities.LoadImage("Delete") }
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
                    // TODO: Read constants from Utilities instead of embedding them directly
                    Utilities.CreateNumericUpDown(this, o => o.Amount),
                    true
                )
            ));
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
