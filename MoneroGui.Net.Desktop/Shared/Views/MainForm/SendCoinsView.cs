using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Controls;
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

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            Utilities.CreateLabel(() => {
                                var balance = Utilities.MoneroRpcManager.AccountManager.Balance;
                                var balanceText = MoneroGUI.Properties.Resources.SendCoinsCurrentBalance + " " + (balance != null ?
                                    MoneroAPI.Utilities.CoinAtomicValueToString(balance.Spendable) :
                                    MoneroGUI.Properties.Resources.PunctuationQuestionMark);

                                return balanceText + " " + MoneroGUI.Properties.Resources.TextCurrencyCode;
                            }),
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
                            Utilities.CreateTextBox(() =>
                                MoneroGUI.Properties.Resources.TextHintOptional,
                                null,
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
                                () => {
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
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.SendCoinsClearRecipients,
                                Utilities.LoadImage("Delete"),
                                () => {
                                    TableLayoutRecipients = new TableLayout(
                                        new CoinSender(),
                                        new TableRow { ScaleHeight = true }
                                    );
                                    ScrollableRecipientsContainer.Content = TableLayoutRecipients;
                                }
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
                                Utilities.LoadImage("Send")
                            )
                        )
                    )
                ) { Spacing = Utilities.Spacing3 }
            );
        }
    }
}
