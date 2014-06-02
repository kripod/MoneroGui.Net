using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jojatekok.MoneroGUI.Views
{
    public partial class DebugConsoleView
    {
        public event EventHandler<string> SendRequested;

        private Logger Logger { get; set; }

        private bool _isAutoScroll = true;
        private bool IsAutoScroll {
            get { return _isAutoScroll; }
            set { _isAutoScroll = value; }
        }

        public DebugConsoleView()
        {
            InitializeComponent();
        }

        private void DebugConsoleView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Logger = DataContext as Logger;
            if (Logger == null) return;

            TextBoxLog.Text = Logger.Messages;
            Logger.OnMessage += Logger_OnMessage;
        }

        private void Logger_OnMessage(object sender, string e)
        {
            Dispatcher.Invoke(() => {
                // TODO: Maintain the scroll bar's position even if the number of max lines have been reached

                var oldVerticalScrollOffset = ScrollViewer.VerticalOffset;
                var oldSelectionLength = TextBoxLog.SelectionLength;
                var newSelectionStart = TextBoxLog.SelectionStart;
                TextBoxLog.Select(0, 0);

                TextBoxLog.Text = Logger.Messages;

                if (oldSelectionLength > 0) {
                    if (Logger.IsMaxLineCountReached) {
                        newSelectionStart -= e.Length;
                    }

                    if (newSelectionStart < 0) newSelectionStart = 0;
                    TextBoxLog.Select(newSelectionStart, oldSelectionLength);
                }

                ScrollViewer.ScrollToVerticalOffset(oldVerticalScrollOffset);
            });
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

        private void TextBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                ButtonSend_Click(null, null);
            }
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            var input = TextBoxInput.Text;
            if (SendRequested != null && !string.IsNullOrWhiteSpace(input)) {
                SendRequested(this, input);
                TextBoxInput.Clear();
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            if (Logger != null) Logger.Clear();

            TextBoxLog.Clear();
            IsAutoScroll = true;
        }
    }
}
