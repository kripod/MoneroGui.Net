using Jojatekok.MoneroAPI;
using System;
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
            var generalSettings = SettingsManager.General;
            ViewModel.TransactionFee = generalSettings.TransactionsDefaultFee;
            ViewModel.MixCount = generalSettings.TransactionsDefaultMixCount;
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
            if (e.OldItems != null || e.Action == NotifyCollectionChangedAction.Reset) {
                RefreshNewEstimatedBalance();
            }
        }

        private void RefreshNewEstimatedBalance()
        {
            var balanceSpendable = ViewModel.BalanceSpendable;
            var transactionFee = ViewModel.TransactionFee;
            if (balanceSpendable == null || transactionFee == null) {
                ViewModel.BalanceNewEstimated = null;
                return;
            }

            var balanceNewEstimated = (double)balanceSpendable.Value - transactionFee.Value;
            var recipients = ViewModel.Recipients;
            for (var i = recipients.Count - 1; i >= 0; i--) {
                var amount = recipients[i].Amount;
                if (amount != null) balanceNewEstimated -= amount.Value;
            }

            ViewModel.BalanceNewEstimated = Math.Round(balanceNewEstimated) / StaticObjects.CoinAtomicValueDivider;
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

        public void ClearRecipients()
        {
            ViewModel.Recipients.Clear();
            AddRecipient();
        }

        private void SendTransaction()
        {
            // Save settings
            SettingsManager.IsAutoSaveEnabled = false;
            var generalSettings = SettingsManager.General;
            if (ViewModel.MixCount == null) ViewModel.MixCount = generalSettings.TransactionsDefaultMixCount;
            if (ViewModel.TransactionFee == null) ViewModel.TransactionFee = generalSettings.TransactionsDefaultFee;
            generalSettings.TransactionsDefaultMixCount = ViewModel.MixCount.Value;
            generalSettings.TransactionsDefaultFee = ViewModel.TransactionFee.Value;
            SettingsManager.IsAutoSaveEnabled = true;
            SettingsManager.SaveSettings();

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
                    ViewModel.MixCount.Value,
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
        }
    }
}
