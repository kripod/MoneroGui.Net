using System.Windows;

namespace Jojatekok.MoneroGUI.Views
{
    public partial class SendCoinsView
    {
        public SendCoinsView()
        {
            InitializeComponent();

            // Load mix count's previous value from settings
            IntegerUpDownMixCount.Value = SettingsManager.General.TransactionsDefaultMixCount;
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

            StaticObjects.MoneroClient.Wallet.Transfer(TextBoxAddress.Text, DoubleUpDownAmount.Value.Value, IntegerUpDownMixCount.Value.Value, TextBoxPaymentId.Text);
            ResetValues();

            SettingsManager.General.TransactionsDefaultMixCount = IntegerUpDownMixCount.Value.Value;
        }

        private void ResetValues()
        {
            TextBoxAddress.Clear();
            TextBoxLabel.Clear();
            DoubleUpDownAmount.Value = 0;
            TextBoxPaymentId.Clear();
        }
    }
}
