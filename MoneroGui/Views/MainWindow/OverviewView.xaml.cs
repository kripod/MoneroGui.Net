using Jojatekok.MoneroGUI.Windows;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    public partial class OverviewView
    {
        public OverviewView()
        {
            InitializeComponent();
        }

        private void OverviewView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) {
                Dispatcher.BeginInvoke(new Action(this.FocusParent), DispatcherPriority.ContextIdle);
            }
        }

        private void ButtonCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ViewModel.Address);
            this.FocusParent();
        }

        private void ButtonQrCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new QrCodeWindow(Window.GetWindow(Parent), ViewModel.Address);
            dialog.ShowDialog();

            this.FocusParent();
        }
    }
}
