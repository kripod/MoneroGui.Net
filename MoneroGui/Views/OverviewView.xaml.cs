using System.Windows;

namespace Jojatekok.MoneroGUI.Views
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
    }
}
