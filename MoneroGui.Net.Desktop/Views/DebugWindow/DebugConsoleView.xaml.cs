using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Jojatekok.MoneroGUI.Views.DebugWindow
{
    public partial class DebugConsoleView
    {
        public event EventHandler<string> SendRequested;

        private Logger Logger { get; set; }

        public DebugConsoleView()
        {
            InitializeComponent();

            this.SetDefaultFocusedElement(TextBoxInput);
        }

        private void DebugConsoleView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Logger = DataContext as Logger;
            if (Logger == null) return;

            TextBoxLog.SetBinding(
                TextBox.TextProperty,
                new Binding {
                    Path = new PropertyPath("Messages"),
                    Source = Logger,
                    IsAsync = true,
                    NotifyOnTargetUpdated = true,
                    Mode = BindingMode.OneWay,
                }
            );
        }

        private void DebugConsoleView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) {
                Dispatcher.BeginInvoke(new Action(() => ScrollViewerLog.ScrollToEnd()), DispatcherPriority.ContextIdle);
            }
        }

// ReSharper disable CompareOfFloatsByEqualityOperator
        private void TextBoxLog_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (ScrollViewerLog.VerticalOffset == ScrollViewerLog.ScrollableHeight) {
                // Auto-scroll
                ScrollViewerLog.ScrollToEnd();

            } else if (Logger.IsMaxLineCountReached) {
                // Maintain the ScrollViewer's previous position (if possible)
                ScrollViewerLog.LineUp();
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

            this.SetFocusedElement(TextBoxInput);
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            if (Logger != null) Logger.Clear();

            this.SetFocusedElement(TextBoxInput);
        }
    }
}
