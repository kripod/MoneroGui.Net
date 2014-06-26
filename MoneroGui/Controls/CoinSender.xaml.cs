using Jojatekok.MoneroGUI.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Jojatekok.MoneroGUI.Controls
{
    public partial class CoinSender
    {
        private SendCoinsRecipient CurrentRecipient { get; set; }

        public CoinSender()
        {
            InitializeComponent();

            this.SetDefaultFocusedElement(TextBoxAddress);
        }

        private void CoinSender_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CurrentRecipient = e.NewValue as SendCoinsRecipient;
            Debug.Assert(CurrentRecipient != null, "CurrentRecipient != null");

            CurrentRecipient.AddressInvalidated += delegate {
                var bindingExpression = TextBoxAddress.GetBindingExpression(TextBox.TextProperty);
                if (bindingExpression != null) bindingExpression.UpdateSource();
            };

            CurrentRecipient.AmountInvalidated += delegate {
                var bindingExpression = DoubleUpDownAmount.GetBindingExpression(DoubleUpDown.ValueProperty);
                if (bindingExpression != null) bindingExpression.UpdateSource();
            };
        }

        private void TextBoxAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            var index = StaticObjects.AddressBookDataSource.IndexOfAddress(TextBoxAddress.Text);
            if (index >= 0) {
                CurrentRecipient.Label = StaticObjects.AddressBookDataSource[index].Label;
            }

            CheckLabelExistence();
        }

        private void TextBoxLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckLabelExistence();
        }

        private void CheckLabelExistence()
        {
            var index = StaticObjects.AddressBookDataSource.IndexOfLabel(TextBoxLabel.Text);
            if (index >= 0 && TextBoxAddress.Text != StaticObjects.AddressBookDataSource[index].Address) {
                TextBoxLabel.Foreground = StaticObjects.BrushForegroundWarning;
            } else {
                TextBoxLabel.Foreground = StaticObjects.BrushForegroundDefault;
            }
        }

        private void ButtonAddressBook_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddressBookWindow(Window.GetWindow(Parent));
            if (dialog.ShowDialog() == true) {
                var selectedContact = dialog.SelectedContact;

                if (selectedContact != null) {
                    CurrentRecipient.Address = selectedContact.Address;
                    CurrentRecipient.Label = selectedContact.Label;

                    this.SetFocusedElement(DoubleUpDownAmount);
                }
            }
        }

        private void ButtonPasteAddress_Click(object sender, RoutedEventArgs e)
        {
            var address = Clipboard.GetText(TextDataFormat.Text);
            if (!string.IsNullOrWhiteSpace(address)) {
                CurrentRecipient.Address = address;
            }

            TextBoxAddress.Select(address.Length, 0);
            this.SetFocusedElement(TextBoxAddress);
        }

        private void ButtonRemoveSelf_Click(object sender, RoutedEventArgs e)
        {
            CurrentRecipient.RemoveSelf();
        }
    }
}
