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
    public class AddressBookView : TableLayout
    {
        public AddressBookView()
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
                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.TextNew,
                                Utilities.LoadImage("Add")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.AddressBookCopyAddress,
                                Utilities.LoadImage("Copy")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.TextEdit,
                                Utilities.LoadImage("Edit")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.TextDelete,
                                Utilities.LoadImage("Delete")
                            )
                        ),

                        new TableCell { ScaleWidth = true },

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.TextQrCode,
                                Utilities.LoadImage("QrCode")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Properties.Resources.TextExport,
                                Utilities.LoadImage("Export")
                            )
                        )
                    )
                ) { Spacing = Utilities.Spacing3 }
            );
        }
    }
}
