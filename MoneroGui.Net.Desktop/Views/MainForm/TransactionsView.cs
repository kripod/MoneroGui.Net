using System;
using System.Collections.Generic;
using Eto;
using Eto.Forms;
using Jojatekok.MoneroAPI;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public class TransactionsView : TableLayout, IExportable
    {
        private GridView GridViewTransactions { get; set; }

        public TransactionsView()
        {
            Spacing = Utilities.Spacing3;

            GridViewTransactions = Utilities.CreateGridView(
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
                    HeaderText = MoneroGUI.Desktop.Properties.Resources.TransactionsTransactionId
                }
            );

            Rows.Add(
                new TableRow(
                    GridViewTransactions
                ) { ScaleHeight = true }
            );

            Rows.Add(
                new TableLayout(
                    new TableRow(
                        new TableCell { ScaleWidth = true },

                        new TableCell(
                            Utilities.CreateButton(() =>
                                MoneroGUI.Desktop.Properties.Resources.TextExport,
                                null,
                                Utilities.LoadImage("Export"),
                                OnButtonExportClick
                            )
                        )
                    )
                )
            );
        }

        private void OnButtonExportClick()
        {
            Export();

            GridViewTransactions.Focus();
        }

        public void Export()
        {
            using (
                var dialog = new SaveFileDialog {
                    Filters = new HashSet<FileDialogFilter> {
                        new FileDialogFilter(MoneroGUI.Desktop.Properties.Resources.TextFilterCsvFiles, Utilities.FileFilterCsv),
                        new FileDialogFilter(MoneroGUI.Desktop.Properties.Resources.TextFilterAllFiles, Utilities.FileFilterAll)
                    },
                    Directory = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                }
            ) {
                if (dialog.ShowDialog(this) != DialogResult.Ok) return;

                Export(dialog.FileName);
            }
        }

        public void Export(string fileName)
        {
            var dataTable = new DataTable();

            var columns = GridViewTransactions.Columns;
            for (var i = 0; i < columns.Count; i++) {
                dataTable.ColumnHeaders.Add(columns[i].HeaderText);
            }

            var dataSource = Utilities.DataSourceAccountTransactions;
            for (var i = 0; i < dataSource.Items.Count; i++) {
                var transaction = dataSource.Items[i];
                dataTable.Rows.Add(
                    new List<object> {
                        transaction.Number,
                        MoneroAPI.Utilities.CoinAtomicValueToDisplayValue(transaction.AmountSpendable),
                        MoneroAPI.Utilities.CoinAtomicValueToDisplayValue(transaction.AmountUnspendable),
                        transaction.TransactionId
                    }
                );
            }
            
            dataTable.ExportToCsvAsync(fileName);
        }
    }
}
