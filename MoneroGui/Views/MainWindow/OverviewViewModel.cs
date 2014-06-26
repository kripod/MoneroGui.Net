using Jojatekok.MoneroAPI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views.MainWindow
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

                if (!string.IsNullOrEmpty(value)) {
                    InitializerVisibility = Visibility.Hidden;
                    AddressVisibility = Visibility.Visible;

                } else {
                    AddressVisibility = Visibility.Hidden;
                    InitializerVisibility = Visibility.Visible;
                }
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

        private ReadOnlyObservableCollection<Transaction> _transactionDataSource;
        public ReadOnlyObservableCollection<Transaction> TransactionDataSource {
            get { return _transactionDataSource; }

            set {
                _transactionDataSource = value;
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
