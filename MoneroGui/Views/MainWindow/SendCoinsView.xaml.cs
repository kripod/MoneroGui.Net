using Jojatekok.MoneroAPI.Objects;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
#endif

            // Check for changes of the total amount in order to determine the new estimated balance
            
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.Recipients.ItemChanged += Recipients_ItemChanged;
            ViewModel.Recipients.CollectionChanged += Recipients_CollectionChanged;

            // Add the first recipient
            AddRecipient();

            // Load settings
            ViewModel.TransactionFee = SettingsManager.General.TransactionsDefaultFee;
            ViewModel.MixCount = SettingsManager.General.TransactionsDefaultMixCount;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BalanceSpendable" || e.PropertyName == "TransactionFee") {
                RefreshNewEstimatedBalance();
            }
        }

        private void Recipients_ItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Amount") {
                RefreshNewEstimatedBalance();
            }
        }

        private void Recipients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Perform a hard refresh if the collection of recipients has been cleared
            if (e.Action == NotifyCollectionChangedAction.Reset) {
                RefreshNewEstimatedBalance();
                return;
            }

            // Check for removed items
            var oldItems = e.OldItems;
            if (oldItems != null && ViewModel.BalanceNewEstimated != null) {
                for (var i = oldItems.Count - 1; i >= 0; i--) {
                    var sendCoinsRecipient = oldItems[i] as SendCoinsRecipient;
                    Debug.Assert(sendCoinsRecipient != null, "sendCoinsRecipient != null");

                    var amount = sendCoinsRecipient.Amount;
                    if (amount != null) {
                        ViewModel.BalanceNewEstimated += amount;
                    }
                }
            }
        }

        // TODO: Instead of "hard refresh", calculate only with the data which is being changed
        private void RefreshNewEstimatedBalance()
        {
            var balanceSpendable = ViewModel.BalanceSpendable;
            if (balanceSpendable == null) {
                ViewModel.BalanceNewEstimated = null;
                return;
            }

            var balanceNewEstimated = balanceSpendable.Value - ViewModel.TransactionFee;
            var recipients = ViewModel.Recipients;
            for (var i = recipients.Count - 1; i >= 0; i--) {
                var amount = recipients[i].Amount;
                if (amount != null) balanceNewEstimated -= amount.Value;
            }

            ViewModel.BalanceNewEstimated = balanceNewEstimated;
        }

        private void AddRecipient()
        {
            var recipient = new SendCoinsRecipient(this);
            ViewModel.Recipients.Add(recipient);

            ListBoxRecipients.ScrollIntoView(recipient);
            ListBoxRecipients.SelectedIndex = ListBoxRecipients.Items.Count - 1;
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
            if (ViewModel.MixCount == null) ViewModel.MixCount = 0;
            if (ViewModel.TransactionFee == null) ViewModel.TransactionFee = 0;

            var recipients = ViewModel.Recipients;
            var recipientsCount = recipients.Count;
            var firstInvalidRecipient = -1;

            var recipientsList = new List<TransferRecipient>(recipientsCount);
            var contactDictionary = new Dictionary<string, string>(recipientsCount);

            // Check each recipient's validity, and add them to a dictionary
            for (var i = recipientsCount - 1; i >= 0; i--) {
                var recipient = recipients[i];

                if (!recipient.IsValid()) {
                    firstInvalidRecipient = i;

                } else if (firstInvalidRecipient < 0) {
                    Debug.Assert(recipient.Amount != null, "recipient.Amount != null");
                    recipientsList.Add(new TransferRecipient(recipient.Address, recipient.Amount.Value));

                    if (!string.IsNullOrWhiteSpace(recipient.Label)) {
                        contactDictionary.Add(recipient.Label, recipient.Address);
                    }
                }
            }

            if (firstInvalidRecipient < 0) {
                // Initiate a new transaction
                var isTransferSuccessful = StaticObjects.MoneroClient.Wallet.SendTransfer(
                    recipientsList,
                    ViewModel.PaymentId,
                    (ulong)ViewModel.MixCount.Value,
                    ViewModel.TransactionFee.Value
                );

                // Add new people to the address book
                foreach (var keyValuePair in contactDictionary) {
                    var contact = new SettingsManager.ConfigElementContact(keyValuePair.Key, keyValuePair.Value);
                    var editIndex = StaticObjects.DataSourceAddressBook.IndexOfLabel(contact.Label);

                    if (editIndex < 0) {
                        StaticObjects.DataSourceAddressBook.Add(contact);
                    } else {
                        StaticObjects.DataSourceAddressBook[editIndex] = contact;
                    }
                }

                if (isTransferSuccessful) {
                    ClearRecipients();
                } else {
                    // Show a warning whether the transaction could not be sent
                    Window.GetWindow(this).ShowError(Properties.Resources.SendCoinsTransactionCouldNotBeSent);
                    this.SetFocusedElement(ListBoxRecipients);
                }

            } else {
                // Notify the user about the first recipient being invalid
                ListBoxRecipients.ScrollIntoView(recipients[firstInvalidRecipient]);
                ListBoxRecipients.SelectedIndex = firstInvalidRecipient;
                this.SetFocusedElement(ListBoxRecipients);
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
            Debug.Assert(ViewModel.MixCount != null, "ViewModel.MixCount != null");
            Debug.Assert(ViewModel.TransactionFee != null, "ViewModel.TransactionFee != null");
            SettingsManager.General.TransactionsDefaultMixCount = ViewModel.MixCount.Value;
            SettingsManager.General.TransactionsDefaultFee = ViewModel.TransactionFee.Value;
        }
    }
}
