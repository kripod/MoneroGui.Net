using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class AddressBookEditWindow
    {
        private IList<SettingsManager.ConfigElementContact> CurrentContacts { get; set; }

        public int OverwriteIndex { get; private set; }

        private int _editIndex = -1;
        private int EditIndex {
            get { return _editIndex; }
            set { _editIndex = value; }
        }

        public string Label {
            get { return TextBoxLabel.Text; }
            private set { TextBoxLabel.Text = value; }
        }

        public string Address {
            get { return TextBoxAddress.Text; }
            private set { TextBoxAddress.Text = value; }
        }

        private AddressBookEditWindow()
        {
            Icon = StaticObjects.ApplicationIcon;
            Loaded += delegate {
                this.SetWindowButtons(false, false);

                MaxHeight = ActualHeight;
                MinHeight = ActualHeight;
            };

            InitializeComponent();
        }

        public AddressBookEditWindow(Window owner, IList<SettingsManager.ConfigElementContact> currentContacts) : this()
        {
            Owner = owner;
            CurrentContacts = currentContacts;

            Title = Properties.Resources.AddressBookEditWindowTitleAdd;
        }

        public AddressBookEditWindow(Window owner, IList<SettingsManager.ConfigElementContact> currentContacts, int editIndex) : this()
        {
            Owner = owner;
            CurrentContacts = currentContacts;

            EditIndex = editIndex;
            if (EditIndex >= 0) {
                Title = Properties.Resources.AddressBookEditWindowTitleEdit;

                var editedContact = CurrentContacts[EditIndex];
                Label = editedContact.Label;
                Address = editedContact.Address;
                TextBoxLabel.Watermark = editedContact.Label;
                TextBoxAddress.Watermark = editedContact.Address;

                TextBoxAddress.SelectAll();
                Dispatcher.BeginInvoke(new Action(() => TextBoxAddress.Focus()), DispatcherPriority.ContextIdle);

            } else {
                Title = Properties.Resources.AddressBookEditWindowTitleAdd;
            }
        }

        private void TextBoxLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            OverwriteIndex = CurrentContacts.IndexOf(Label);

            if (OverwriteIndex >= 0 && OverwriteIndex != EditIndex) {
                // Notify the user of the overwriting operation
                TextBoxLabel.Foreground = StaticObjects.BrushForegroundWarning;

            } else {
                TextBoxLabel.Foreground = StaticObjects.BrushForegroundDefault;
            }

            CheckInputsValidity();
        }

        private void TextBoxAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckInputsValidity();
        }

        private void CheckInputsValidity()
        {
            ButtonOk.IsEnabled = Label.Length > 0 && Address.Length > 0;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
