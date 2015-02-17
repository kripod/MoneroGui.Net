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
        private const int ColumnPaddingExtra = 30;

        public OverviewView()
        {
            Spacing = new Size(ColumnPaddingExtra, 0);

            Rows.Add(
                new TableRow(
                    new TableCell(
                        Utilities.CreateLabel(
                            () => MoneroGUI.Properties.Resources.TextAccount,
                            new Font(SystemFont.Default, Utilities.FontSizeTitle, FontDecoration.Underline)
                        ),
                        true
                    ),

                    new TableCell(
                        Utilities.CreateLabel(
                            () => MoneroGUI.Properties.Resources.OverviewRecentTransactions,
                            new Font(SystemFont.Default, Utilities.FontSizeTitle, FontDecoration.Underline)
                        ),
                        true
                    )
                )
            );

            var spacing = new Size(Utilities.PaddingSmall, Utilities.PaddingSmall);

            Rows.Add(
                new TableRow(
                    new TableCell(
                        new TableLayout {
                            Padding = new Padding(Utilities.PaddingExtraLarge, Utilities.PaddingMedium),
                            Spacing = spacing,
                            Rows = {
                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.OverviewSpendable),
                                            true
                                        ),
                                        new TableCell(
                                            // TODO
                                            Utilities.CreateLabel(() => "?" + " " + MoneroGUI.Properties.Resources.TextCurrencyCode)
                                        )
                                    ),

                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.OverviewUnconfirmed),
                                            true
                                        ),
                                        new TableCell(
                                            // TODO
                                            Utilities.CreateLabel(() => "?" + " " + MoneroGUI.Properties.Resources.TextCurrencyCode)
                                        )
                                    )
                                ) { Spacing = spacing },

                                new Separator(SeparatorOrientation.Horizontal),

                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.TextAddress + MoneroGUI.Properties.Resources.PunctuationColon),
                                            true
                                        ),
                                        new TableCell(
                                            // TODO
                                            Utilities.CreateLabel(() => "?")
                                        )
                                    )
                                ) { Spacing = spacing },

                                new Separator(SeparatorOrientation.Horizontal),

                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.OverviewNumberOfTransactions),
                                            true
                                        ),
                                        new TableCell(
                                            // TODO
                                            Utilities.CreateLabel(() => "?")
                                        )
                                    )
                                ) { Spacing = spacing }
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
