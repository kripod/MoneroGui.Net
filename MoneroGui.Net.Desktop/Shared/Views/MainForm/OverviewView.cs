using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Controls;

namespace Jojatekok.MoneroGUI.Views.MainForm
{
    public class OverviewView : TableLayout
    {
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
                            new Font(SystemFont.Default, Utilities.FontSizeTitle, FontDecoration.Underline)
                        ),
                        true
                    ),

                    new TableCell(
                        Utilities.CreateLabel(
                            () => MoneroGUI.Properties.Resources.OverviewRecentTransactions,
                            HorizontalAlign.Left,
                            VerticalAlign.Middle,
                            new Font(SystemFont.Default, Utilities.FontSizeTitle, FontDecoration.Underline)
                        ),
                        true
                    )
                )
            );

            var labelAccountBalanceSpendable = Utilities.CreateLabel(() =>
                {
                    var balance = Utilities.MoneroRpcManager.AccountManager.Balance;
                    var balanceText = balance != null ?
                        MoneroAPI.Utilities.CoinAtomicValueToDisplayValue(balance.Spendable).ToString(Utilities.InvariantCulture) :
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
                        MoneroAPI.Utilities.CoinAtomicValueToDisplayValue(balance.Unconfirmed).ToString(Utilities.InvariantCulture) :
                        MoneroGUI.Properties.Resources.PunctuationQuestionMark;

                    return balanceText + " " + MoneroGUI.Properties.Resources.TextCurrencyCode;
                },
                HorizontalAlign.Right
            );
            Utilities.BindingsToAccountBalance.Add(labelAccountBalanceUnconfirmed.Bindings[0]);

            var labelAccountAddress = Utilities.CreateLabel(() =>
                Utilities.MoneroRpcManager.AccountManager.Address ?? MoneroGUI.Properties.Resources.PunctuationQuestionMark,
                HorizontalAlign.Right
            );
            Utilities.BindingsToAccountAddress.Add(labelAccountAddress.Bindings[0]);

            var labelAccountTransactionsCount = Utilities.CreateLabel(() =>
                Utilities.MoneroRpcManager.AccountManager.Transactions.Count.ToString(Utilities.InvariantCulture),
                HorizontalAlign.Right,
                VerticalAlign.Top
            );
            Utilities.BindingsToAccountTransactions.Add(labelAccountTransactionsCount.Bindings[0]);

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
                                        new TableCell(
                                            labelAccountAddress,
                                            true
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
