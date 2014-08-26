using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class BackupManagerDeterministicAccountSeedWindow
    {
        private BackupManagerDeterministicAccountSeedWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();
        }

        public BackupManagerDeterministicAccountSeedWindow(Window owner, string mnemonicKey) : this()
        {
            Owner = owner;
            TextBoxDeterministicAccountSeed.Text = mnemonicKey;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
