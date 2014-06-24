using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Jojatekok.MoneroGUI.Windows;

namespace Jojatekok.MoneroGUI.Controls
{
    public partial class CoinSender
    {
        public CoinSender()
        {
            InitializeComponent();
        }

        private void CoinSender_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) {
                Dispatcher.BeginInvoke(new Action(() => TextBoxAddress.Focus()), DispatcherPriority.ContextIdle);
            }
        }

        private void ButtonAddressBook_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddressBookWindow(Window.GetWindow(Parent));
            if (dialog.ShowDialog() == true) {
                var selectedContact = dialog.SelectedContact;

                if (selectedContact != null) {
                    var dataContext = DataContext as SendCoinsRecipient;
                    Debug.Assert(dataContext != null, "dataContext != null");

                    dataContext.Address = selectedContact.Address;
                    dataContext.Label = selectedContact.Label;

                    FocusManager.SetFocusedElement(this, DoubleUpDownAmount);
                }
            }
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
