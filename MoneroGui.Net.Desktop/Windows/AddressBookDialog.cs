using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Views.MainForm;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class AddressBookDialog : Dialog<SettingsManager.ConfigElementContact>
    {
        public AddressBookDialog()
        {
            this.SetWindowProperties(
                MoneroGUI.Desktop.Properties.Resources.TextAddressBook,
                new Size(400, 250)
            );

            RenderContent();
        }

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            Content = new AddressBookView { IsDialogModeEnabled = true };
        }
    }
}
