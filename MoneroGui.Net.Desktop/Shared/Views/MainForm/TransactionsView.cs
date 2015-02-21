using Eto;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using System.Collections.ObjectModel;

namespace Jojatekok.MoneroGUI.Views.MainForm
{
    public class TransactionsView : TableLayout
    {
        public TransactionsView()
        {
            Spacing = Utilities.Spacing3;

            var gridViewTransactions = Utilities.CreateGridView(
                new ReadOnlyObservableCollection<Transaction>(Utilities.AccountTransactions),
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<Transaction, string>(o => o.Number.ToString(Utilities.InvariantCulture)) },
                    HeaderText = "#"
                },
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<Transaction, string>(o => o.AmountSpendable.ToString(Utilities.InvariantCulture)) },
                    HeaderText = "Spendable" // TODO: Localization
                },
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<Transaction, string>(o => o.AmountUnspendable.ToString(Utilities.InvariantCulture)) },
                    HeaderText = "Not spendable" // TODO: Localization
                },
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Property<Transaction, string>(o => o.TransactionId) },
                    HeaderText = MoneroGUI.Properties.Resources.TransactionsTransactionId
                }
            );

            Rows.Add(
                new TableRow(
                    gridViewTransactions
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
