using Jojatekok.MoneroAPI;
using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    public partial class TransactionsView : IExportable
    {
        public TransactionsView()
        {
            InitializeComponent();

            this.SetDefaultFocusedElement(DataGridTransactions);
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Export();

            this.SetFocusedElement(DataGridTransactions);
        }

        public void Export()
        {
            var dialog = new SaveFileDialog {
                Filter = Properties.Resources.TextFilterCsvFiles + "|" + Properties.Resources.TextFilterAllFiles,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() == true) Export(dialog.FileName);
        }

        public void Export(string fileName)
        {
            using (var dataTable = new DataTable { Locale = Helper.InvariantCulture }) {
                var columns = DataGridTransactions.Columns;
                for (var i = 0; i < columns.Count; i++) {
                    dataTable.Columns.Add(columns[i].Header.ToString());
                }

                for (var i = 0; i < DataGridTransactions.Items.Count; i++) {
                    var transaction = DataGridTransactions.Items[i] as Transaction;
                    Debug.Assert(transaction != null, "transaction != null");

                    var amountDisplayValue = transaction.Amount / StaticObjects.CoinAtomicValueDivider;
                    dataTable.Rows.Add(
                        transaction.Number,
                        transaction.Type,
                        transaction.IsAmountSpendable,
                        amountDisplayValue.ToString("G", Helper.DefaultUiCulture),
                        "=\"" + transaction.TransactionId + "\""
                    );
                }

                dataTable.ExportToCsvAsync(fileName);
            }
        }
    }
}
