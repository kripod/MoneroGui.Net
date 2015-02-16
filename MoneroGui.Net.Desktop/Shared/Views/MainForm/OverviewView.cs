using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;

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

            Rows.Add(
                new TableRow(
                    new TableCell(
                        new TableLayout {
                            Padding = new Padding(Utilities.PaddingExtraLarge, Utilities.PaddingMedium),
                            Rows = {
                                new TableRow(
                                    new TableCell(
                                        Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.OverviewSpendable),
                                        true
                                    ),
                                    new TableCell(
                                        Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.OverviewSpendable)
                                    )
                                )
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
