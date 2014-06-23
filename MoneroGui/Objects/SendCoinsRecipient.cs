using Jojatekok.MoneroGUI.Views.MainWindow;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI
{
    public class SendCoinsRecipient : INotifyPropertyChanged
    {
        private SendCoinsView Owner { get; set; }

        private string _address;
        public string Address {
            get { return _address; }

            set {
                _address = value;
                OnPropertyChanged();
            }
        }

        private string _label;
        public string Label {
            get { return _label; }

            set {
                _label = value;
                OnPropertyChanged();
            }
        }

        private double? _amount = 0;
        public double? Amount {
            get { return _amount; }

            set {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public SendCoinsRecipient(SendCoinsView owner)
        {
            Owner = owner;
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Address)) return false;

            if (Amount == null || Amount.Value <= 0) {
                return false;
            }

            return true;
        }

        public void RemoveSelf()
        {
            Owner.RemoveRecipient(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
