using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    sealed class StatusBarViewModel : INotifyPropertyChanged
    {
        private Visibility _syncBarVisibility = Visibility.Hidden;
        public Visibility SyncBarVisibility {
            get { return _syncBarVisibility; }

            private set {
                if (value == _syncBarVisibility) return;

                _syncBarVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _syncBarText;
        public string SyncBarText {
            get { return _syncBarText; }

            set {
                _syncBarText = value;
                OnPropertyChanged();
            }
        }

        private ulong _blocksDownloaded;
        public ulong BlocksDownloaded {
            get { return _blocksDownloaded; }

            set {
                _blocksDownloaded = value;
                OnPropertyChanged();

                SyncBarVisibility = BlocksDownloaded == BlocksTotal ?
                                    Visibility.Hidden :
                                    Visibility.Visible;
            }
        }

        private ulong _blocksTotal;
        public ulong BlocksTotal {
            get { return _blocksTotal; }

            set {
                if (value == _blocksTotal) return;

                _blocksTotal = value;
                OnPropertyChanged();
            }
        }

        private byte _connectionCount;
        public byte ConnectionCount {
            get { return _connectionCount; }

            set {
                _connectionCount = value;
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
