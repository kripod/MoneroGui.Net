using System;
using System.Diagnostics;
using System.Windows;

namespace Jojatekok.MoneroGUI.Controls
{
    public partial class CoinSender
    {
        public CoinSender()
        {
            InitializeComponent();
        }

        private void ButtonAddressBook_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonPasteAddress_Click(object sender, RoutedEventArgs e)
        {
            var address = Clipboard.GetText(TextDataFormat.Text);
            if (!string.IsNullOrWhiteSpace(address)) {
                Debug.Assert(DataContext as SendCoinsRecipient != null, "DataContext as SendCoinsRecipient != null");
                (DataContext as SendCoinsRecipient).Address = address;
            }

            TextBoxAddress.Select(address.Length, 0);
            TextBoxAddress.Focus();
        }

        private void ButtonRemoveSelf_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(DataContext as SendCoinsRecipient != null, "DataContext as SendCoinsRecipient != null");
            (DataContext as SendCoinsRecipient).RemoveSelf();
        }
    }
}
