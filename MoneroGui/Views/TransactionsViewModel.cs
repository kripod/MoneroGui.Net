using Jojatekok.MoneroAPI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Views
{
    sealed class TransactionsViewModel : INotifyPropertyChanged
    {
        private ReadOnlyObservableCollection<Transaction> _dataSource;
        public ReadOnlyObservableCollection<Transaction> DataSource {
            get { return _dataSource; }

            set {
                if (value == _dataSource) return;

                _dataSource = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
