using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI.Views
{
    public partial class DebugConsoleView
    {
        private bool _isAutoScroll = true;
        private bool IsAutoScroll {
            get { return _isAutoScroll; }
            set { _isAutoScroll = value; }
        }

        public DebugConsoleView()
        {
            InitializeComponent();
        }

// ReSharper disable CompareOfFloatsByEqualityOperator
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0) {
                IsAutoScroll = ScrollViewer.VerticalOffset == ScrollViewer.ScrollableHeight;

            } else if (IsAutoScroll) {
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
            }
        }
// ReSharper restore CompareOfFloatsByEqualityOperator

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(DataContext != null, "DataContext != null");

            var logger = DataContext as Logger;
            if (logger != null) logger.Clear();

            IsAutoScroll = true;
        }
    }
}
