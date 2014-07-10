using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    sealed class SendCoinsViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollectionEx<SendCoinsRecipient> _recipients = new ObservableCollectionEx<SendCoinsRecipient>();
        public ObservableCollectionEx<SendCoinsRecipient> Recipients {
            get { return _recipients; }
        }

        private bool _isSendingEnabled;
        public bool IsSendingEnabled {
            get { return _isSendingEnabled; }

            private set {
                _isSendingEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isBlockchainSynced;
        public bool IsBlockchainSynced {
            private get { return _isBlockchainSynced; }

            set {
                _isBlockchainSynced = value;
                OnPropertyChanged();

                UpdatePropertyIsSendingEnabled();
            }
        }

        private ulong? _balanceSpendable;
        public ulong? BalanceSpendable {
            get { return _balanceSpendable; }

            set {
                _balanceSpendable = value;
                OnPropertyChanged();

                UpdatePropertyIsSendingEnabled();
            }
        }

        private double? _balanceNewEstimated;
        public double? BalanceNewEstimated {
            get { return _balanceNewEstimated; }

            set {
                _balanceNewEstimated = value;
                OnPropertyChanged();
            }
        }

        private string _paymentId;
        public string PaymentId {
            get { return _paymentId; }

            set {
                _paymentId = value;
                OnPropertyChanged();
            }
        }

        private ulong? _mixCount;
        public ulong? MixCount {
            get { return _mixCount; }
            set {
                _mixCount = value;
                OnPropertyChanged();
            }
        }

        private ulong? _transactionFee;
        public ulong? TransactionFee {
            get { return _transactionFee; }

            set {
                _transactionFee = value;
                OnPropertyChanged();
            }
        }

        private void UpdatePropertyIsSendingEnabled()
        {
            IsSendingEnabled = IsBlockchainSynced && BalanceSpendable != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
