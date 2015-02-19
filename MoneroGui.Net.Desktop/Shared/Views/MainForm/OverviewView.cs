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
                            VerticalAlign.Middle,
                            new Font(SystemFont.Default, Utilities.FontSizeTitle, FontDecoration.Underline)
                        ),
                        true
                    ),

                    new TableCell(
                        Utilities.CreateLabel(
                            () => MoneroGUI.Properties.Resources.OverviewRecentTransactions,
                            VerticalAlign.Middle,
                            new Font(SystemFont.Default, Utilities.FontSizeTitle, FontDecoration.Underline)
                        ),
                        true
                    )
                )
            );

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
                                ) { Spacing = Utilities.Spacing2 },

                                new Separator(SeparatorOrientation.Horizontal),

                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() =>
                                                MoneroGUI.Properties.Resources.TextAddress + MoneroGUI.Properties.Resources.PunctuationColon,
                                                VerticalAlign.Top
                                            ),
                                            true
                                        ),
                                        new TableCell(
                                            // TODO
                                            Utilities.CreateLabel(() => "?")
                                        )
                                    )
                                ) { Spacing = Utilities.Spacing2 },

                                new Separator(SeparatorOrientation.Horizontal),

                                new TableLayout(
                                    new TableRow(
                                        new TableCell(
                                            Utilities.CreateLabel(() =>
                                                MoneroGUI.Properties.Resources.OverviewNumberOfTransactions,
                                                VerticalAlign.Top
                                            ),
                                            true
                                        ),
                                        new TableCell(
                                            // TODO
                                            Utilities.CreateLabel(() =>
                                                "?",
                                                VerticalAlign.Top
                                            )
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
