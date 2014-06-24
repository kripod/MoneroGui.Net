using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    public partial class SendCoinsView
    {
        public SendCoinsView()
        {
            InitializeComponent();

#if DEBUG
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
#endif

            // Add the first recipient
            AddRecipient();

            // Load settings
            IntegerUpDownMixCount.Value = SettingsManager.General.TransactionsDefaultMixCount;
        }

        private void AddRecipient()
        {
            ViewModel.Recipients.Add(new SendCoinsRecipient(this));
        }

        public void RemoveRecipient(SendCoinsRecipient item)
        {
            var recipients = ViewModel.Recipients;
            recipients.Remove(item);

            if (recipients.Count == 0) {
                AddRecipient();
            }
        }

        private void ClearRecipients()
        {
            ViewModel.Recipients.Clear();
            AddRecipient();
        }

        private void SendTransaction()
        {
            if (IntegerUpDownMixCount.Value == null) {
                IntegerUpDownMixCount.Value = 0;
            }

            var recipients = ViewModel.Recipients;
            var recipientsDictionary = new Dictionary<string, double>(recipients.Count);
            
            for (var i = recipients.Count - 1; i >= 0; i--) {
                var recipient = recipients[i];

                if (!recipient.IsValid()) {
                    // TODO: Notify the user about a recipient being invalid
                    return;

                } else {
                    Debug.Assert(recipient.Amount != null, "recipient.Amount != null");
                    recipientsDictionary.Add(recipient.Address, recipient.Amount.Value);
                }
            }

            StaticObjects.MoneroClient.Wallet.Transfer(recipientsDictionary, IntegerUpDownMixCount.Value.Value, TextBoxPaymentId.Text);
            ClearRecipients();
        }

        private void ButtonAddRecipient_Click(object sender, RoutedEventArgs e)
        {
            AddRecipient();

            ListBoxRecipients.SelectedIndex = ListBoxRecipients.Items.Count - 1;
            ListBoxRecipients.Focus();
        }

        private void ButtonClearRecipients_Click(object sender, RoutedEventArgs e)
        {
            ClearRecipients();

            ListBoxRecipients.Focus();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            SendTransaction();

            // Save settings
            Debug.Assert(IntegerUpDownMixCount.Value != null, "IntegerUpDownMixCount.Value != null");
            SettingsManager.General.TransactionsDefaultMixCount = IntegerUpDownMixCount.Value.Value;

            ListBoxRecipients.Focus();
        }
    }
}
