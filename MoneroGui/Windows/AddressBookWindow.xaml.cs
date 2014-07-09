using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class AddressBookWindow
    {
        public SettingsManager.ConfigElementContact SelectedContact {
            get { return AddressBookView.DataGridAddressBook.SelectedItem as SettingsManager.ConfigElementContact; }
        }

        private AddressBookWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;
            SourceInitialized += delegate {
                this.SetWindowButtons(false, true);

                MaxWidth = ActualWidth;
                MinWidth = ActualWidth;
            };
            ContentRendered += delegate {
                SizeToContent = SizeToContent.Manual;
                MaxWidth = double.PositiveInfinity;
            };

            InitializeComponent();
        }

        public AddressBookWindow(Window owner) : this()
        {
            Owner = owner;
        }
    }
}
