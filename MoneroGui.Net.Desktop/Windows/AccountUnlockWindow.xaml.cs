using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class AccountUnlockWindow
    {
        public string Passphrase {
            get { return PasswordBoxPassphrase.Password; }
        }

        private AccountUnlockWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;
            SourceInitialized += delegate {
                this.SetWindowButtons(false, false);

                MinWidth = ActualWidth;
                MaxHeight = ActualHeight;
                MinHeight = ActualHeight;
            };

            InitializeComponent();
        }

        public AccountUnlockWindow(Window owner) : this()
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
