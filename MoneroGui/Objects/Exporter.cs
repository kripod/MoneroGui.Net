using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI
{
    static class Exporter
    {
        private const string CsvDelimiter = ";";

        public static async void ExportToCsvAsync(this DataTable dataTable, string fileName)
        {
            using (var stream = new StreamWriter(fileName, false, Encoding.UTF8, 4096)) {
                var columnCount = dataTable.Columns.Count;
                var columnCountMinus1 = columnCount - 1;

                // Write the headers
                for (var i = 0; i < columnCount; i++) {
                    await stream.WriteAsync(dataTable.Columns[i].ToString());
                    if (i < columnCountMinus1) {
                        await stream.WriteAsync(CsvDelimiter);
                    }
                }

                // Write all the rows
                for (var i = 0; i < dataTable.Rows.Count; i++) {
                    await stream.WriteLineAsync();

                    for (var j = 0; j < columnCount; j++) {
                        await stream.WriteAsync(dataTable.Rows[i][j].ToString());

                        if (j < columnCountMinus1) {
                            await stream.WriteAsync(CsvDelimiter);
                        }
                    }
                }
            }
        }
    }
}
