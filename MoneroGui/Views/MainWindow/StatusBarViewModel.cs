using System.Windows;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    sealed class StatusBarViewModel : DependencyObject
    {
        private const string ConnectionCountIndicatorImageUriBase = "/Resources/Images/ConnectionCountIndicator{0}.png";

        public static readonly DependencyProperty SyncStatusVisibilityProperty = DependencyProperty.RegisterAttached(
            "SyncStatusVisibility",
            typeof(Visibility),
            typeof(StatusBarViewModel),
            new PropertyMetadata(Visibility.Hidden)
        );

        public static readonly DependencyProperty SyncBarProgressPercentageProperty = DependencyProperty.RegisterAttached(
            "SyncBarProgressPercentage",
            typeof(double),
            typeof(StatusBarViewModel)
        );

        public static readonly DependencyProperty SyncBarTextProperty = DependencyProperty.RegisterAttached(
            "SyncBarText",
            typeof(string),
            typeof(StatusBarViewModel)
        );

        public static readonly DependencyProperty ConnectionCountProperty = DependencyProperty.RegisterAttached(
            "ConnectionCount",
            typeof(ulong),
            typeof(StatusBarViewModel)
        );

        public static readonly DependencyProperty ConnectionCountIndicatorImageUriProperty = DependencyProperty.RegisterAttached(
            "ConnectionCountIndicatorImageUri",
            typeof(string),
            typeof(StatusBarViewModel),
            new PropertyMetadata(string.Format(Helper.InvariantCulture, ConnectionCountIndicatorImageUriBase, 0))
        );

        private bool _isSyncStatusShowable = true;
        public Visibility SyncStatusVisibility {
            get { return (Visibility)GetValue(SyncStatusVisibilityProperty); }

            set {
                if (!_isSyncStatusShowable) return;
                if (value == Visibility.Hidden) _isSyncStatusShowable = false;

                SetValue(SyncStatusVisibilityProperty, value);
            }
        }

        public double SyncBarProgressPercentage {
            get { return (double)GetValue(SyncBarProgressPercentageProperty); }
            set { SetValue(SyncBarProgressPercentageProperty, value); }
        }

        public string SyncBarText {
            get { return GetValue(SyncBarTextProperty) as string; }
            set { SetValue(SyncBarTextProperty, value); }
        }

        public ulong ConnectionCount {
            get { return (ulong)GetValue(ConnectionCountProperty); }

            set {
                SetValue(ConnectionCountProperty, value);

                if (value == 0) {
                    ConnectionCountIndicatorIndex = 0;
                } else if (value < 4) {
                    ConnectionCountIndicatorIndex = 1;
                } else if (value < 7) {
                    ConnectionCountIndicatorIndex = 2;
                } else if (value < 10) {
                    ConnectionCountIndicatorIndex = 3;
                } else {
                    ConnectionCountIndicatorIndex = 4;
                }
            }
        }

        private byte _connectionCountIndicatorIndex;
        private byte ConnectionCountIndicatorIndex {
            set {
                if (value == _connectionCountIndicatorIndex) return;

                _connectionCountIndicatorIndex = value;
                ConnectionCountIndicatorImageUri = string.Format(Helper.InvariantCulture, ConnectionCountIndicatorImageUriBase, value);
            }
        }

        public string ConnectionCountIndicatorImageUri {
            get { return GetValue(ConnectionCountIndicatorImageUriProperty) as string; }
            private set { SetValue(ConnectionCountIndicatorImageUriProperty, value); }
        }
    }
}
