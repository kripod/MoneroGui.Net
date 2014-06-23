using Jojatekok.MoneroGUI.Windows;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    public partial class OverviewView
    {
        public OverviewView()
        {
            InitializeComponent();
        }

        private void ButtonCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ViewModel.Address);
        }

        private void ButtonQrCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new QrCodeWindow(Window.GetWindow(Parent), ViewModel.Address);
            dialog.ShowDialog();
        }
    }
}
