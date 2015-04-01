using Eto;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using System;
using System.Collections.Generic;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public class TransactionsView : TableLayout, IExportable
    {
        private static readonly FilterCollection<Transaction> DataSourceAccountTransactions = new FilterCollection<Transaction>(Utilities.DataSourceAccountTransactions) {
            Sort = (x, y) => y.Index.CompareTo(x.Index)
        };

        private GridView GridViewTransactions { get; set; }

        public TransactionsView()
        {
            Spacing = Utilities.Spacing3;

            GridViewTransactions = Utilities.CreateGridView(
                DataSourceAccountTransactions,
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
                    HeaderText = Desktop.Properties.Resources.TransactionsTransactionId
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
                                Desktop.Properties.Resources.TextExport,
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
            using (var dialog = new SaveFileDialog { Directory = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) }) {
                dialog.Filters.Add(new FileDialogFilter(Desktop.Properties.Resources.TextFilterCsvFiles, Utilities.FileFilterCsv));
                dialog.Filters.Add(new FileDialogFilter(Desktop.Properties.Resources.TextFilterAllFiles, Utilities.FileFilterAll));

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
            for (var i = 0; i < dataSource.Count; i++) {
                var transaction = dataSource[i];
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
