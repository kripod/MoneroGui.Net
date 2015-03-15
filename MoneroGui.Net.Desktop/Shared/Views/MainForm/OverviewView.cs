using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Controls;
using Jojatekok.MoneroGUI.Windows;

namespace Jojatekok.MoneroGUI.Views.MainForm
{
    public class OverviewView : TableLayout
    {
        private Button ButtonShowAddressQrCode { get; set; }
        private Panel PanelCopyAddress { get; set; }

        public OverviewView()
        {
            Spacing = new Size(Utilities.Padding7, 0);

            Rows.Add(
                new TableRow(
                    new TableCell(
                        Utilities.CreateLabel(
                            () => MoneroGUI.Properties.Resources.TextAccount,
                            HorizontalAlign.Left,
                            VerticalAlign.Middle,
                            new Font(SystemFont.Default, Utilities.FontSize2, FontDecoration.Underline)
                        ),
                        true
                    ),

                    new TableCell(
                        Utilities.CreateLabel(
                            () => MoneroGUI.Properties.Resources.OverviewRecentTransactions,
                            HorizontalAlign.Left,
                            VerticalAlign.Middle,
                            new Font(SystemFont.Default, Utilities.FontSize2, FontDecoration.Underline)
                        ),
                        true
                    )
                )
            );

            var labelAccountBalanceSpendable = Utilities.CreateLabel(() =>
                {
                    var balance = Utilities.MoneroRpcManager.AccountManager.Balance;
                    var balanceText = balance != null ?
                        MoneroAPI.Utilities.CoinAtomicValueToString(balance.Spendable) :
                        MoneroGUI.Properties.Resources.PunctuationQuestionMark;

                    return balanceText + " " + MoneroGUI.Properties.Resources.TextCurrencyCode;
                },
                HorizontalAlign.Right
            );
            Utilities.BindingsToAccountBalance.Add(labelAccountBalanceSpendable.Bindings[0]);

            var labelAccountBalanceUnconfirmed = Utilities.CreateLabel(() =>
                {
                    var balance = Utilities.MoneroRpcManager.AccountManager.Balance;
                    var balanceText = balance != null ?
                        MoneroAPI.Utilities.CoinAtomicValueToString(balance.Unconfirmed) :
                        MoneroGUI.Properties.Resources.PunctuationQuestionMark;

                    return balanceText + " " + MoneroGUI.Properties.Resources.TextCurrencyCode;
                },
                HorizontalAlign.Right
            );
            Utilities.BindingsToAccountBalance.Add(labelAccountBalanceUnconfirmed.Bindings[0]);

            var labelAccountAddress = Utilities.CreateLabel(() =>
                {
                    var address = Utilities.MoneroRpcManager.AccountManager.Address;
                    if (address == null) return MoneroGUI.Properties.Resources.OverviewInitializing;

                    ButtonShowAddressQrCode.Visible = true;
                    PanelCopyAddress.Visible = true;
                    return Utilities.MoneroRpcManager.AccountManager.Address;
                },
                HorizontalAlign.Right
            );
            // TODO: labelAccountAddress.Wrap = WrapMode.Character;
            Utilities.BindingsToAccountAddress.Add(labelAccountAddress.Bindings[0]);

            var labelAccountTransactionsCount = Utilities.CreateLabel(() =>
                Utilities.DataSourceAccountTransactions.Count.ToString(Utilities.InvariantCulture),
                HorizontalAlign.Right,
                VerticalAlign.Top
            );
            Utilities.BindingsToAccountTransactions.Add(labelAccountTransactionsCount.Bindings[0]);

            var buttonCopyAddress = Utilities.CreateButton(
                null,
                () => MoneroGUI.Properties.Resources.TextCopy,
                Utilities.LoadImage("Copy"),
                delegate { Utilities.Clipboard.Text = Utilities.MoneroRpcManager.AccountManager.Address; }
            );

            ButtonShowAddressQrCode = Utilities.CreateButton(
                null,
                () => MoneroGUI.Properties.Resources.TextQrCode,
                Utilities.LoadImage("QrCode"),
                delegate {
                    using (var dialog = new QrCodeDialog(Utilities.MoneroRpcManager.AccountManager.Address)) {
                        dialog.ShowModal(this);
                    }
                }
            );
            ButtonShowAddressQrCode.Visible = false;

            PanelCopyAddress = new Panel {
                Content = buttonCopyAddress,
                Visible = false,
                Padding = new Padding(Utilities.Padding2, 0)
            };

            Rows.Add(
                new TableRow(
                    new TableCell(
                        new TableLayout {
                            Padding = new Padding(Utilities.Padding6, Utilities.Padding3),
                            Spacing = Utilities.Spacing5,
                            Rows = {
                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.OverviewSpendable)
                                        ),
                                        new TableCell(
                                            labelAccountBalanceSpendable,
                                            true
                                        )
                                    ),

                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.OverviewUnconfirmed)
                                        ),
                                        new TableCell(
                                            labelAccountBalanceUnconfirmed,
                                            true
                                        )
                                    )
                                ) { Spacing = Utilities.Spacing2 },

                                new Separator(SeparatorOrientation.Horizontal),

                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() =>
                                                MoneroGUI.Properties.Resources.TextAddress + MoneroGUI.Properties.Resources.PunctuationColon,
                                                HorizontalAlign.Left,
                                                VerticalAlign.Top
                                            )
                                        ),
                                        new TableRow(
                                            new TableCell(
                                                labelAccountAddress,
                                                true
                                            ),
                                            PanelCopyAddress,
                                            ButtonShowAddressQrCode
                                        )
                                    )
                                ) { Spacing = Utilities.Spacing2 },

                                new Separator(SeparatorOrientation.Horizontal),

                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() =>
                                                MoneroGUI.Properties.Resources.OverviewNumberOfTransactions,
                                                HorizontalAlign.Left,
                                                VerticalAlign.Top
                                            )
                                        ),
                                        new TableCell(
                                            labelAccountTransactionsCount,
                                            true
                                        )
                                    )
                                ) { Spacing = Utilities.Spacing2 }
                            }
                        },
                        true
                    )

                    // TODO: Show recent transactions
                    //new TableCell(
                    //    null,
                    //    true
                    //)
                ) { ScaleHeight = true }
            );
        }
    }
}
