using Jojatekok.MoneroGUI.Views.MainWindow;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI
{
    public class SendCoinsRecipient : INotifyPropertyChanged
    {
        internal event EventHandler AddressInvalidated;
        internal event EventHandler AmountInvalidated;

        private SendCoinsView Owner { get; set; }

        private static readonly ValidationRuleAddress AddressValidator = new ValidationRuleAddress();
        private static readonly ValidationRuleDoubleBiggerThanZero AmountValidator = new ValidationRuleDoubleBiggerThanZero();

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
            var output = true;

            if (!AddressValidator.Validate(Address, Helper.InvariantCulture).IsValid) {
                if (AddressInvalidated != null) AddressInvalidated(this, EventArgs.Empty);
                output = false;
            }

            if (!AmountValidator.Validate(Amount, Helper.InvariantCulture).IsValid) {
                if (AmountInvalidated != null) AmountInvalidated(this, EventArgs.Empty);
                output = false;
            }

            return output;
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
