using Eto.Drawing;
using Eto.Forms;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Controls
{
    public sealed class CoinSender : TableLayout, ICloneable, INotifyPropertyChanged
    {
        private double _amount;

        private readonly TextBox _textBoxAddress = Utilities.CreateTextBox(() =>
            MoneroGUI.Properties.Resources.TextHintAddress,
            null,
            new Font(FontFamilies.Monospace, Utilities.FontSize1)
        );
        private readonly TextBox _textBoxLabel = Utilities.CreateTextBox(() =>
            MoneroGUI.Properties.Resources.TextHintLabel
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
                    MoneroGUI.Properties.Resources.TextAddress +
                    MoneroGUI.Properties.Resources.PunctuationColon
                ),

                new TableCell(TextBoxAddress, true),

                new Button { Image = Utilities.LoadImage("Contact") },
                new Button { Image = Utilities.LoadImage("Paste") },
                new Button { Image = Utilities.LoadImage("Delete") }
            ));

            Rows.Add(new TableRow(
                Utilities.CreateLabel(() =>
                    MoneroGUI.Properties.Resources.TextLabel +
                    MoneroGUI.Properties.Resources.PunctuationColon
                ),

                new TableCell(TextBoxLabel, true)
            ));

            Rows.Add(new TableRow(
                Utilities.CreateLabel(() =>
                    MoneroGUI.Properties.Resources.TextAmount +
                    MoneroGUI.Properties.Resources.PunctuationColon
                ),

                new TableCell(
                    // TODO: Read constants from Utilities instead of embedding them directly
                    Utilities.CreateNumericUpDown(12, 0.001, 0, double.MaxValue, this, o => o.Amount),
                    true
                )
            ));
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
