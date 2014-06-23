using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    sealed class SendCoinsViewModel : INotifyPropertyChanged
    {
        private static readonly ObservableCollection<SendCoinsRecipient> RecipientsPrivate = new ObservableCollection<SendCoinsRecipient>();
        public ObservableCollection<SendCoinsRecipient> Recipients {
            get { return RecipientsPrivate; }
        }

        private bool _isSendingEnabled;
        public bool IsSendingEnabled {
            get { return _isSendingEnabled; }

            set {
                _isSendingEnabled = value;
                OnPropertyChanged();
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
