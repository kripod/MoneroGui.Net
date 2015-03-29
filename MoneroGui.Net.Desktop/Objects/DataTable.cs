using System.Collections.Generic;

namespace Jojatekok.MoneroGUI.Desktop
{
    class DataTable
    {
        private readonly IList<string> _columnHeaders = new List<string>();
        private readonly IList<IList<object>> _rows = new List<IList<object>>();

        public IList<string> ColumnHeaders { get { return _columnHeaders; } }
        public IList<IList<object>> Rows { get { return _rows; } }
    }
}
