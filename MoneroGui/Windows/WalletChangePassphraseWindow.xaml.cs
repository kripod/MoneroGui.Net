using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class WalletChangePassphraseWindow
    {
        private bool IsCurrentPassphraseRequired { get; set; }

        private string CurrentPassphrase {
            get { return PasswordBoxCurrentPassphrase.Password; }
        }

        public string NewPassphrase {
            get { return PasswordBoxNewPassphrase1.Password; }
        }

        private WalletChangePassphraseWindow()
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

        public WalletChangePassphraseWindow(Window owner, bool isCurrentPassphraseRequired) : this()
        {
            Owner = owner;
            IsCurrentPassphraseRequired = isCurrentPassphraseRequired;

            string baseNewPassphraseText;
            if (IsCurrentPassphraseRequired) {
                baseNewPassphraseText = Properties.Resources.WalletChangePassphraseWindowNewPassphrase;
                Title = Properties.Resources.WalletChangePassphraseWindowTitleChangePassphrase;

            } else {
                baseNewPassphraseText = Properties.Resources.WalletChangePassphraseWindowPassphrase;
                Title = Properties.Resources.WalletChangePassphraseWindowTitleEncryptWallet;
                TextBlockCurrentPassphrase.Visibility = Visibility.Collapsed;
                PasswordBoxCurrentPassphrase.Visibility = Visibility.Collapsed;
            }

            TextBlockNewPassphrase1.Text = baseNewPassphraseText + ":";
            TextBlockNewPassphrase2.Text = string.Format(
                Helper.InvariantCulture,
                "{0} ({1}):",
                baseNewPassphraseText,
                Properties.Resources.WalletChangePassphraseWindowAgain
            );
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckInputsValidity();
        }

        private void CheckInputsValidity()
        {
            if (IsCurrentPassphraseRequired && CurrentPassphrase.Length == 0) {
                ButtonOk.IsEnabled = false;
                return;
            }

            if (PasswordBoxNewPassphrase1.Password.Length == 0 || PasswordBoxNewPassphrase2.Password.Length == 0) {
                ButtonOk.IsEnabled = false;
                return;
            }

            ButtonOk.IsEnabled = true;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBoxNewPassphrase1.Password != PasswordBoxNewPassphrase2.Password) {
                this.ShowError(Properties.Resources.WalletChangePassphraseWindowPassphrasesDoNotMatch);

                PasswordBoxNewPassphrase1.SelectAll();
                this.SetFocusedElement(PasswordBoxNewPassphrase1);
                return;
            }

            // TODO: Check for the current passphrase's validity, and display the following text on error:
            // The passphrase entered for the wallet's decryption is incorrect.

            DialogResult = true;
        }
    }
}
