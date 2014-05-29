using Jojatekok.MoneroGUI.Windows;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views
{
    public partial class SendCoinsView
    {
        public SendCoinsView()
        {
            InitializeComponent();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            if (DoubleUpDownAmount.Value == null || DoubleUpDownAmount.Value == 0) {
                // TODO: Show message "insufficient funds"
                return;
            }

            if (IntegerUpDownMixCount.Value == null) {
                IntegerUpDownMixCount.Value = 0;
            }

            MainWindow.MoneroClient.Wallet.Transfer(IntegerUpDownMixCount.Value.Value, TextBoxAddress.Text, DoubleUpDownAmount.Value.Value);
        }
    }
}
