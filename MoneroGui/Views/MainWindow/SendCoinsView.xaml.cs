using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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

            ListBoxRecipients.SelectedIndex = ListBoxRecipients.Items.Count - 1;
            SetFocusToSelectedRecipient();
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
            var transactionDictionary = new Dictionary<string, double>(recipients.Count);
            var contactDictionary = new Dictionary<string, string>(recipients.Count);
            var firstInvalidRecipient = -1;

            // Check each recipient's validity, and add them to a dictionary
            for (var i = recipients.Count - 1; i >= 0; i--) {
                var recipient = recipients[i];

                if (!recipient.IsValid()) {
                    firstInvalidRecipient = i;

                } else if (firstInvalidRecipient < 0) {
                    Debug.Assert(recipient.Amount != null, "recipient.Amount != null");
                    transactionDictionary.Add(recipient.Address, recipient.Amount.Value);

                    if (!string.IsNullOrWhiteSpace(recipient.Label)) {
                        contactDictionary.Add(recipient.Label, recipient.Address);
                    }
                }
            }

            if (firstInvalidRecipient < 0) {
                // Initiate a new transaction
                StaticObjects.MoneroClient.Wallet.Transfer(transactionDictionary, IntegerUpDownMixCount.Value.Value, TextBoxPaymentId.Text);

                // Add new people to the address book
                foreach (var keyValuePair in contactDictionary) {
                    var contact = new SettingsManager.ConfigElementContact(keyValuePair.Key, keyValuePair.Value);
                    var editIndex = StaticObjects.AddressBookDataSource.IndexOfLabel(contact.Label);

                    if (editIndex < 0) {
                        StaticObjects.AddressBookDataSource.Add(contact);
                    } else {
                        StaticObjects.AddressBookDataSource[editIndex] = contact;
                    }
                }

                ClearRecipients();

            } else {
                // Notify the user about the first recipient being invalid
                ListBoxRecipients.ScrollIntoView(recipients[firstInvalidRecipient]);
            }
        }

        private void ButtonAddRecipient_Click(object sender, RoutedEventArgs e)
        {
            AddRecipient();
        }

        private void ButtonClearRecipients_Click(object sender, RoutedEventArgs e)
        {
            ClearRecipients();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            SendTransaction();

            // Save settings
            Debug.Assert(IntegerUpDownMixCount.Value != null, "IntegerUpDownMixCount.Value != null");
            SettingsManager.General.TransactionsDefaultMixCount = IntegerUpDownMixCount.Value.Value;
        }

        private void SetFocusToSelectedRecipient()
        {
            var itemContainer = ListBoxRecipients.ItemContainerGenerator.ContainerFromIndex(ListBoxRecipients.SelectedIndex) as ListBoxItem;
            if (itemContainer == null) return;
            itemContainer.Focus();
        }
    }
}
