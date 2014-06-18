using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class AddressBookAddWindow
    {
        public AddressBookAddWindow()
        {
            Icon = StaticObjects.ApplicationIcon;
            Loaded += delegate {
                this.SetWindowButtons(false, false);

                MaxHeight = ActualHeight;
                MinHeight = ActualHeight;
            };

            InitializeComponent();
        }

        public AddressBookAddWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Check fields and throw error messages if needed

            DialogResult = true;
            Close();
        }
    }
}
