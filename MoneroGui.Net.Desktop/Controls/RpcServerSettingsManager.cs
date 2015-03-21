using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Desktop.Controls
{
    public sealed class RpcServerSettingsManager : GroupBox, INotifyPropertyChanged
    {
        private ushort _rpcUrlPort;

        private TextBox TextBoxRpcUrlHost { get; set; }
        private CheckBox CheckBoxIsProcessHostedLocally { get; set; }

        public string RpcUrlHost {
            get { return TextBoxRpcUrlHost.Text; }
            set { TextBoxRpcUrlHost.Text = value; }
        }

        public ushort RpcUrlPort {
            get { return _rpcUrlPort; }
            set {
                _rpcUrlPort = value;
                OnPropertyChanged();
            }
        }

        public bool IsProcessHostedLocally {
            get { return CheckBoxIsProcessHostedLocally.Checked == true; }
            set { CheckBoxIsProcessHostedLocally.Checked = value; }
        }

        public RpcServerSettingsManager(string title, string rpcUrlHost, ushort rpcUrlPort, bool isProcessHostedLocally)
        {
            Text = title;

            TextBoxRpcUrlHost = new TextBox { Text = rpcUrlHost };
            CheckBoxIsProcessHostedLocally = new CheckBox {
                Text = MoneroGUI.Desktop.Properties.Resources.OptionsNetworkIsProcessHostedLocally,
                Checked = isProcessHostedLocally
            };

            RpcUrlPort = rpcUrlPort;

            Content = new TableLayout(
                new TableLayout(
                    new TableRow(
                        new Label { Text = MoneroGUI.Desktop.Properties.Resources.TextHost },
                        new TableCell(TextBoxRpcUrlHost, true),

                        new Separator(SeparatorOrientation.Vertical),

                        new Label { Text = MoneroGUI.Desktop.Properties.Resources.TextPort },
                        Utilities.CreateNumericUpDown(this, o => o.RpcUrlPort, 0, 1, ushort.MaxValue)
                    )
                ) { Spacing = Utilities.Spacing3 },

                new TableRow(CheckBoxIsProcessHostedLocally)
            ) { Padding = new Padding(Utilities.Padding2), Spacing = Utilities.Spacing3 };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
