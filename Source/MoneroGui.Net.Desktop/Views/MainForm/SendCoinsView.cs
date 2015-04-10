using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroGUI.Desktop.Controls;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public sealed class SendCoinsView : TableLayout
    {
        public static readonly SendCoinsViewModel ViewModel = new SendCoinsViewModel();

        private TableLayout TableLayoutRecipients { get; set; }
        private Scrollable ScrollableRecipientsContainer { get; set; }

        public SendCoinsView()
        {
            Spacing = Utilities.Spacing3;

            DataContext = ViewModel;

            var buttonSendTransaction = Utilities.CreateButton(() =>
                Desktop.Properties.Resources.TextSend,
                null,
                Utilities.LoadImage("Send"),
                SendTransaction
            );
            buttonSendTransaction.BindDataContext<bool>("Enabled", "IsSendingEnabled");

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            Utilities.CreateLabel(ViewModel, o => o.BalanceSpendableText),
                            true
                        ),

                        new TableCell(
                            Utilities.CreateLabel(() =>
                                Desktop.Properties.Resources.SendCoinsEstimatedNewBalance + " " +
                                Desktop.Properties.Resources.PunctuationQuestionMark
                            )
                        )
                    )
                ) { Spacing = Utilities.Spacing3 }
            );

            TableLayoutRecipients = new TableLayout(
                new CoinSender(),
                new TableRow { ScaleHeight = true }
            );
            ScrollableRecipientsContainer = new Scrollable {
                Content = TableLayoutRecipients,
                Padding = new Padding(Utilities.Padding4, 0),
                BackgroundColor = Colors.White
            };

            Rows.Add(
                new TableRow(ScrollableRecipientsContainer) { ScaleHeight = true }
            );

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            Utilities.CreateLabel(() => Desktop.Properties.Resources.TextPaymentId)
                        ),

                        new TableCell(
                            Utilities.CreateTextBox(
                                ViewModel,
                                o => o.PaymentId,
                                () => Desktop.Properties.Resources.TextHintOptional
                            ),
                            true
                        )
                    )
                ) { Spacing = Utilities.Spacing3 }
            );

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            Utilities.CreateButton(() =>
                                Desktop.Properties.Resources.SendCoinsAddRecipient,
                                null,
                                Utilities.LoadImage("Add"),
                                AddRecipient
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                Desktop.Properties.Resources.SendCoinsClearRecipients,
                                null,
                                Utilities.LoadImage("Delete"),
                                Clear
                            )
                        ),

                        new TableCell { ScaleWidth = true },

                        new TableCell(
                            Utilities.CreateLabel(() => Desktop.Properties.Resources.SendCoinsMixCount)
                        ),

                        new TableCell(
                            Utilities.CreateNumericUpDown(ViewModel, o => o.MixCount, 0, 1, ushort.MaxValue)
                        ),

                        new TableCell(
                            new Separator(SeparatorOrientation.Vertical)
                        ),

                        new TableCell(
                            buttonSendTransaction
                        )
                    )
                ) { Spacing = Utilities.Spacing3 }
            );
        }

        void AddRecipient()
        {
            var oldTableLayoutRows = TableLayoutRecipients.Rows;
            var newTableLayout = new TableLayout();

            for (var i = 0; i < oldTableLayoutRows.Count - 1; i++) {
                var coinSender = oldTableLayoutRows[i].Cells[0].Control as CoinSender;
                Debug.Assert(coinSender != null, "coinSender != null");
                newTableLayout.Rows.Add(coinSender.Clone() as CoinSender);
            }

            newTableLayout.Rows.Add(new CoinSender());
            newTableLayout.Rows.Add(new TableRow { ScaleHeight = true });

            TableLayoutRecipients = newTableLayout;
            ScrollableRecipientsContainer.Content = TableLayoutRecipients;
            ScrollableRecipientsContainer.ScrollPosition = new Point(0, int.MaxValue);
        }

        void Clear()
        {
            TableLayoutRecipients = new TableLayout(
                new CoinSender(),
                new TableRow { ScaleHeight = true }
            );
            ScrollableRecipientsContainer.Content = TableLayoutRecipients;

            ViewModel.PaymentId = "";
        }

        void SendTransaction()
        {
            // Disable auto-saving settings
            SettingsManager.IsAutoSaveEnabled = false;
            SettingsManager.General.TransactionsDefaultMixCount = ViewModel.MixCount;

            var tableLayoutRecipientsRows = TableLayoutRecipients.Rows;
            var recipientsCount = tableLayoutRecipientsRows.Count - 1;
            var isExchangeSendWithoutPaymentIdQuestionShowable = ViewModel.PaymentId.Length == 0;

            var recipientsList = new List<TransferRecipient>(recipientsCount);
            var contactDictionary = new Dictionary<string, string>(recipientsCount);

            // Check each recipient's validity, and add them to a dictionary
            for (var i = recipientsCount - 1; i >= 0; i--) {
                var recipient = tableLayoutRecipientsRows[i].Cells[0].Control as CoinSender;
                Debug.Assert(recipient != null, "recipient != null");

                if (!recipient.IsRecipientValid()) {
                    // TODO: Notify the user about the first recipient being invalid
                    //ListBoxRecipients.ScrollIntoView(recipients[firstInvalidRecipient]);
                    //ListBoxRecipients.SelectedIndex = firstInvalidRecipient;
                    //this.SetFocusedElement(ListBoxRecipients);
                    this.ShowError(Desktop.Properties.Resources.SendCoinsTransactionCouldNotBeSent);
                    return;
                }

                // Check whether the address belongs to an exchange
                if (isExchangeSendWithoutPaymentIdQuestionShowable && Utilities.DataSourceExchangeAddresses.Contains(recipient.Address)) {
                    if (this.ShowQuestion(Desktop.Properties.Resources.SendCoinsExchangeSendWithoutPaymentIdQuestionMessage, Desktop.Properties.Resources.SendCoinsExchangeSendWithoutPaymentIdQuestionTitle)) {
                        return;
                    }
                    isExchangeSendWithoutPaymentIdQuestionShowable = false;
                }

                recipientsList.Add(new TransferRecipient(recipient.Address, recipient.Amount));

                if (recipient.Label.Length != 0) {
                    contactDictionary.Add(recipient.Label, recipient.Address);
                }
            }

            // Check for the validity of the payment ID
            if (!MoneroAPI.Utilities.IsPaymentIdValid(ViewModel.PaymentId)) {
                this.ShowError(Desktop.Properties.Resources.SendCoinsInvalidPaymentId);
                return;
            }

            // Initiate a new transaction
            var isTransferSuccessful = Utilities.MoneroRpcManager.AccountManager.SendTransaction(
                recipientsList,
                ViewModel.PaymentId.ToLower(Utilities.InvariantCulture),
                ViewModel.MixCount
            );

            // Add new people to the address book
            foreach (var keyValuePair in contactDictionary) {
                var contact = new SettingsManager.ConfigElementContact(keyValuePair.Key, keyValuePair.Value);
                var editIndex = Utilities.DataSourceAddressBook.IndexOfLabel(contact.Label);

                if (editIndex < 0) {
                    Utilities.DataSourceAddressBook.Add(contact);
                } else {
                    Utilities.DataSourceAddressBook.RemoveAt(editIndex);
                    Utilities.DataSourceAddressBook.Insert(editIndex, contact);
                }
            }

            // Save settings
            SettingsManager.IsAutoSaveEnabled = true;
            SettingsManager.SaveSettings();

            if (isTransferSuccessful) {
                // TODO: Notify the user about the transaction's success
                Application.Instance.AsyncInvoke(Clear);
            } else {
                // Show a warning whether the transaction could not be sent
                this.ShowError(Desktop.Properties.Resources.SendCoinsTransactionCouldNotBeSent);
            }
        }
    }
}
