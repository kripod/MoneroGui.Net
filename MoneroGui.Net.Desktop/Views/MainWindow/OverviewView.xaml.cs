using Jojatekok.MoneroGUI.Windows;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    public partial class OverviewView
    {
        public OverviewView()
        {
            InitializeComponent();

            this.SetDefaultFocusToParent();
        }

        private void ButtonCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ViewModel.Address);

            this.SetFocusToParent();
        }

        private void ButtonQrCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new QrCodeWindow(Window.GetWindow(Parent), ViewModel.Address);
            dialog.ShowDialog();

            this.SetFocusToParent();
        }
    }
}
