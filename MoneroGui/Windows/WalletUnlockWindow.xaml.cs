using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class WalletUnlockWindow
    {
        public string Passphrase {
            get { return PasswordBoxPassphrase.Password; }
        }

        private WalletUnlockWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;
            Loaded += delegate {
                this.SetWindowButtons(false, false);

                MinWidth = ActualWidth;
                MaxHeight = ActualHeight;
                MinHeight = ActualHeight;
            };

            InitializeComponent();
        }

        public WalletUnlockWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private void PasswordBoxPassphrase_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckInputsValidity();
        }

        private void CheckInputsValidity()
        {
            ButtonOk.IsEnabled = Passphrase.Length != 0;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
