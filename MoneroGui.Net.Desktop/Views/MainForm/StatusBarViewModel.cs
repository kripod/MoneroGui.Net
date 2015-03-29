using Eto.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public class StatusBarViewModel : INotifyPropertyChanged
    {
        private bool _isBlockchainSynced;
        private bool _isSyncBarVisible;
        private int _syncBarProgressValue;
        private string _syncBarText;
        private string _syncStatusText = Properties.Resources.StatusBarStatusSynchronizing;
        private string _syncStatusIndicatorText;
        private ulong _connectionCount;
        private string _connectionCountText = "0 " + Properties.Resources.StatusBarConnections;
        private Image _syncStatusIndicatorImage = Utilities.LoadImage("Synchronize");
        private Image _connectionCountIndicatorImage = Utilities.LoadImage("ConnectionCountIndicator0");

        public bool IsBlockchainSynced {
            get { return _isBlockchainSynced; }
            set {
                _isBlockchainSynced = value;

                if (value) {
                    IsSyncBarVisible = false;
                    SyncStatusText = Properties.Resources.StatusBarStatusUpToDate;
                    SyncStatusIndicatorImage = Utilities.LoadImage("Ok");
                } else {
                    SyncStatusText = Properties.Resources.StatusBarStatusSynchronizing;
                }
            }
        }

        public bool IsSyncBarVisible {
            get { return _isSyncBarVisible; }
            set {
                if (value == _isSyncBarVisible) return;

                _isSyncBarVisible = value;
                OnPropertyChanged();
            }
        }

        public int SyncBarProgressValue {
            get { return _syncBarProgressValue; }
            set {
                _syncBarProgressValue = value;
                OnPropertyChanged();
            }
        }

        public string SyncBarText {
            get { return _syncBarText; }
            set {
                _syncBarText = value;
                OnPropertyChanged();
            }
        }

        public string SyncStatusText {
            get { return _syncStatusText; }
            set {
                _syncStatusText = value;
                OnPropertyChanged();
            }
        }

        public string SyncStatusIndicatorText {
            get { return _syncStatusIndicatorText; }
            set {
                _syncStatusIndicatorText = value;
                OnPropertyChanged();
            }
        }

        public ulong ConnectionCount {
            get { return _connectionCount; }
            set {
                _connectionCount = value;
                ConnectionCountText = value + " " + Properties.Resources.StatusBarConnections;

                byte connectionCountIndicatorImageIndex;
                if (value == 0) {
                    connectionCountIndicatorImageIndex = 0;
                } else if (value < 4) {
                    connectionCountIndicatorImageIndex = 1;
                } else if (value < 7) {
                    connectionCountIndicatorImageIndex = 2;
                } else if (value < 10) {
                    connectionCountIndicatorImageIndex = 3;
                } else {
                    connectionCountIndicatorImageIndex = 4;
                }
                ConnectionCountIndicatorImage = Utilities.LoadImage("ConnectionCountIndicator" + connectionCountIndicatorImageIndex);
            }
        }

        public string ConnectionCountText {
            get { return _connectionCountText; }
            set {
                _connectionCountText = value;
                OnPropertyChanged();
            }
        }

        public Image SyncStatusIndicatorImage {
            get { return _syncStatusIndicatorImage; }
            set {
                _syncStatusIndicatorImage = value;
                OnPropertyChanged();
            }
        }

        public Image ConnectionCountIndicatorImage {
            get { return _connectionCountIndicatorImage; }
            set {
                _connectionCountIndicatorImage = value;
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
