using System;
using System.Windows;
using System.Windows.Threading;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    sealed class StatusBarViewModel : DependencyObject
    {
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
            set { SetValue(ConnectionCountProperty, value); }
        }
    }
}
