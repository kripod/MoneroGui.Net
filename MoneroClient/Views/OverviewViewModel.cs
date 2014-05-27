using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Jojatekok.MoneroClient.Views
{
    sealed class OverviewViewModel : INotifyPropertyChanged
    {
        private Visibility _addressVisibility = Visibility.Hidden;
        public Visibility AddressVisibility {
            get { return _addressVisibility; }

            private set {
                _addressVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _initializerVisibility = Visibility.Visible;
        public Visibility InitializerVisibility {
            get { return _initializerVisibility; }

            private set {
                _initializerVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address {
            get { return _address; }

            set {
                _address = value;
                OnPropertyChanged();

                InitializerVisibility = Visibility.Hidden;
                AddressVisibility = Visibility.Visible;
            }
        }

        private double? _balanceSpendable;
        public double? BalanceSpendable {
            get { return _balanceSpendable; }

            set {
                _balanceSpendable = value;
                OnPropertyChanged();
            }
        }

        private double? _balanceUnconfirmed;
        public double? BalanceUnconfirmed {
            get { return _balanceUnconfirmed; }

            set {
                _balanceUnconfirmed = value;
                OnPropertyChanged();
            }
        }

        private ulong _transactionCount;
        public ulong TransactionCount {
            get { return _transactionCount; }

            set {
                _transactionCount = value;
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
