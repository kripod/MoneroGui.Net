using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroGUI.Controls;

namespace Jojatekok.MoneroGUI.Views.MainForm
{
    public class SendCoinsViewModel : INotifyPropertyChanged
    {
        // TODO: Add a list of transfer recipients

        private bool _isSendingEnabled;
        private bool _isBlockchainSynced;
        private ulong? _balanceSpendable;
        private double? _balanceNewEstimated;
        private string _paymentId = string.Empty;
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
