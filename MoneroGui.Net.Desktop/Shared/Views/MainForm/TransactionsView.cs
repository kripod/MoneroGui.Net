using Eto;
using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI.Views.MainForm
{
    public class TransactionsView : TableLayout
    {
        public TransactionsView()
        {
            Spacing = Utilities.Spacing3;

            Rows.Add(
                new TableRow(
                    new GridView<SendCoinsViewModel>()
                ) { ScaleHeight = true }
            );

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell { ScaleWidth = true },

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.TextExport,
                                Utilities.LoadImage("Export")
                            )
                        )
                    )
                )
            );
        }
    }
}
