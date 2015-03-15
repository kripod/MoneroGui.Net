using Eto;
using Eto.Forms;
using Jojatekok.MoneroAPI;

namespace Jojatekok.MoneroGUI.Views.MainForm
{
    public class TransactionsView : TableLayout
    {
        public TransactionsView()
        {
            Spacing = Utilities.Spacing3;

            var gridViewTransactions = Utilities.CreateGridView(
                Utilities.DataSourceAccountTransactions,
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<Transaction, string>(o => o.Number.ToString(Utilities.InvariantCulture)) },
                    HeaderText = "#"
                },
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<Transaction, string>(o => MoneroAPI.Utilities.CoinAtomicValueToString(o.AmountSpendable)) },
                    HeaderText = "Spendable" // TODO: Localization
                },
                new GridColumn {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<Transaction, string>(o => MoneroAPI.Utilities.CoinAtomicValueToString(o.AmountUnspendable)) },
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
                                null,
                                Utilities.LoadImage("Export")
                            )
                        )
                    )
                )
            );
        }
    }
}
