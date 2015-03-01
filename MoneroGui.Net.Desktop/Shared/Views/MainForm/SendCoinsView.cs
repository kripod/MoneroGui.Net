using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroGUI.Controls;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Views.MainForm
{
    public class SendCoinsView : TableLayout
    {
        private static readonly SendCoinsViewModel ViewModel = new SendCoinsViewModel();

        private TableLayout TableLayoutRecipients { get; set; }
        private Scrollable ScrollableRecipientsContainer { get; set; }

        public SendCoinsView()
        {
            Spacing = Utilities.Spacing3;

            var labelCurrentBalance = Utilities.CreateLabel(() => {
                var balance = Utilities.MoneroRpcManager.AccountManager.Balance;
                var balanceText = MoneroGUI.Properties.Resources.SendCoinsCurrentBalance + " " + (balance != null ?
                    MoneroAPI.Utilities.CoinAtomicValueToString(balance.Spendable) :
                    MoneroGUI.Properties.Resources.PunctuationQuestionMark);

                return balanceText + " " + MoneroGUI.Properties.Resources.TextCurrencyCode;
            });
            Utilities.BindingsToAccountBalance.Add(labelCurrentBalance.Bindings[0]);

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            labelCurrentBalance,
                            true
                        ),

                        new TableCell(
                            Utilities.CreateLabel(() =>
                                MoneroGUI.Properties.Resources.SendCoinsEstimatedNewBalance + " " +
                                "?"
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
                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.TextPaymentId)
                        ),

                        new TableCell(
                            Utilities.CreateTextBox(
                                ViewModel,
                                o => o.PaymentId,
                                () => MoneroGUI.Properties.Resources.TextHintOptional,
                                new Font(FontFamilies.Monospace, Utilities.FontSize1)
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
                                MoneroGUI.Properties.Resources.SendCoinsAddRecipient,
                                Utilities.LoadImage("Add"),
                                AddRecipient
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.SendCoinsClearRecipients,
                                Utilities.LoadImage("Delete"),
                                ClearRecipients
                            )
                        ),

                        new TableCell { ScaleWidth = true },

                        new TableCell(
                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.SendCoinsMixCount)
                        ),

                        new TableCell(
                            Utilities.CreateNumericUpDown(0, 1, 0, ushort.MaxValue, ViewModel, o => o.MixCount)
                        ),

                        new TableCell(
                            new Separator(SeparatorOrientation.Vertical)
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.TextSend,
                                Utilities.LoadImage("Send"),
                                SendTransaction
                            )
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

        void ClearRecipients()
        {
            TableLayoutRecipients = new TableLayout(
                new CoinSender(),
                new TableRow { ScaleHeight = true }
            );
            ScrollableRecipientsContainer.Content = TableLayoutRecipients;
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
                    break;

                }

                // Check whether the address belongs to an exchange
                if (isExchangeSendWithoutPaymentIdQuestionShowable && Utilities.DataSourceExchangeAddresses.Contains(recipient.Address)) {
                    if (MessageBox.Show(this, MoneroGUI.Properties.Resources.SendCoinsExchangeSendWithoutPaymentIdQuestionMessage, MoneroGUI.Properties.Resources.SendCoinsExchangeSendWithoutPaymentIdQuestionTitle, MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes) == DialogResult.Yes) {
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
                MessageBox.Show(this, MoneroGUI.Properties.Resources.SendCoinsInvalidPaymentId, MessageBoxType.Error);
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
                    Utilities.DataSourceAddressBook[editIndex] = contact;
                }
            }

            // Save settings
            SettingsManager.IsAutoSaveEnabled = true;
            SettingsManager.SaveSettings();

            if (isTransferSuccessful) {
                // TODO: Notify the user about the transaction's success
                ClearRecipients();
            } else {
                // Show a warning whether the transaction could not be sent
                MessageBox.Show(this, MoneroGUI.Properties.Resources.SendCoinsTransactionCouldNotBeSent, MessageBoxType.Error);
            }
        }
    }
}
