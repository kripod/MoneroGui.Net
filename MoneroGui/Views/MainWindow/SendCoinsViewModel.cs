using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    sealed class SendCoinsViewModel : INotifyPropertyChanged
    {
        private bool _isSendingEnabled;
        public bool IsSendingEnabled {
            get { return _isSendingEnabled; }

            set {
                _isSendingEnabled = value;
                OnPropertyChanged();
            }
        }

        private double _coinBalance;
        public double CoinBalance {
            get { return _coinBalance; }

            set {
                _coinBalance = value;
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
