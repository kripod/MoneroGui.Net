using Eto;
using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public class AddressBookView : TableLayout
    {
        public AddressBookView()
        {
            Spacing = Utilities.Spacing3;

            Rows.Add(
                new TableRow(
                    Utilities.CreateGridView(
                        Utilities.DataSourceAddressBook,
                        new GridColumn {
                            DataCell = new TextBoxCell { Binding = Binding.Property<SettingsManager.ConfigElementContact, string>(o => o.Label) },
                            HeaderText = "Label" // TODO: Localize
                        },
                        new GridColumn {
                            DataCell = new TextBoxCell { Binding = Binding.Property<SettingsManager.ConfigElementContact, string>(o => o.Address) },
                            HeaderText = "Address" // TODO: Localize
                        }
                    )
                ) { ScaleHeight = true }
            );

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Desktop.Properties.Resources.TextNew,
                                null,
                                Utilities.LoadImage("Add")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Desktop.Properties.Resources.AddressBookCopyAddress,
                                null,
                                Utilities.LoadImage("Copy")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Desktop.Properties.Resources.TextEdit,
                                null,
                                Utilities.LoadImage("Edit")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Desktop.Properties.Resources.TextDelete,
                                null,
                                Utilities.LoadImage("Delete")
                            )
                        ),

                        new TableCell { ScaleWidth = true },

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Desktop.Properties.Resources.TextQrCode,
                                null,
                                Utilities.LoadImage("QrCode")
                            )
                        ),

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Desktop.Properties.Resources.TextExport,
                                null,
                                Utilities.LoadImage("Export")
                            )
                        )
                    )
                ) { Spacing = Utilities.Spacing3 }
            );
        }
    }
}
