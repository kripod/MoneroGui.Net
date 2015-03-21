using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public class SendCoinsViewModel : INotifyPropertyChanged
    {
        private bool _isSendingEnabled;
        private bool _isBlockchainSynced;
        private ulong? _balanceSpendable;
        private double? _balanceNewEstimated;
        private string _paymentId = "";
        private ulong _mixCount = 3;

        public bool IsSendingEnabled {
            get { return _isSendingEnabled; }
            private set {
                _isSendingEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsBlockchainSynced {
            private get { return _isBlockchainSynced; }
            set {
                _isBlockchainSynced = value;
                OnPropertyChanged();

                //UpdatePropertyIsSendingEnabled();
            }
        }

        public ulong? BalanceSpendable {
            get { return _balanceSpendable; }
            set {
                _balanceSpendable = value;
                OnPropertyChanged();

                //UpdatePropertyIsSendingEnabled();
            }
        }

        public double? BalanceNewEstimated {
            get { return _balanceNewEstimated; }
            set {
                _balanceNewEstimated = value;
                OnPropertyChanged();
            }
        }

        public string PaymentId {
            get { return _paymentId; }
            set {
                _paymentId = value;
                OnPropertyChanged();
            }
        }

        public ulong MixCount {
            get { return _mixCount; }
            set {
                _mixCount = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
