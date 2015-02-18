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
    public class SendCoinsView : TableLayout
    {
        public SendCoinsView()
        {
            var spacing = new Size(Utilities.PaddingMedium, Utilities.PaddingMedium);
            Spacing = spacing;

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            Utilities.CreateLabel(() =>
                                MoneroGUI.Properties.Resources.SendCoinsCurrentBalance + " " +
                                "?"
                            ),
                            true
                        ),

                        new TableCell(
                            Utilities.CreateLabel(() =>
                                MoneroGUI.Properties.Resources.SendCoinsEstimatedNewBalance + " " +
                                "?"
                            )
                        )
                    )
                ) { Spacing = spacing }
            );

            var listBoxRecipients = new ListBox();

            Rows.Add(new TableRow(listBoxRecipients) { ScaleHeight = true });

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
                                new Font(FontFamilies.Monospace, 10)
                            ),
                            true
                        )
                    )
                ) { Spacing = spacing }
            );

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.SendCoinsAddRecipient,
                                Utilities.LoadImage("Add")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.SendCoinsClearRecipients,
                                Utilities.LoadImage("Delete")
                            )
                        ),

                        new TableCell { ScaleWidth = true },

                        new TableCell(
                            Utilities.CreateLabel(() => MoneroGUI.Properties.Resources.SendCoinsMixCount)
                        ),

                        new TableCell(
                            Utilities.CreateNumericUpDown()
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
                ) { Spacing = spacing }
            );
        }
    }
}
