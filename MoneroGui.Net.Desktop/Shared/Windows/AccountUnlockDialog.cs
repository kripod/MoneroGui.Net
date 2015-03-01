using Eto.Drawing;
using Eto.Forms;

namespace Jojatekok.MoneroGUI.Forms
{
    public sealed class AccountUnlockDialog : Dialog<string>
    {
        public AccountUnlockDialog()
        {
            this.SetWindowProperties(
                MoneroGUI.Properties.Resources.AccountUnlockWindowTitle,
                new Size(300, 0)
            );

            RenderContent();
        }

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            var passwordBoxResult = new PasswordBox();

            var buttonOk = new Button { Text = MoneroGUI.Properties.Resources.TextOk };
            buttonOk.Click += delegate { Close(passwordBoxResult.Text); };
            DefaultButton = buttonOk;

            var buttonCancel = new Button { Text = MoneroGUI.Properties.Resources.TextCancel };
            AbortButton = buttonCancel;

            Content = new TableLayout(
                new Label { Text = MoneroGUI.Properties.Resources.AccountUnlockWindowInstruction },

                passwordBoxResult,

                new TableRow(
                    new TableLayout(
                        new TableRow(
                            new TableCell { ScaleWidth = true },
                            new TableCell(buttonOk),
                            new TableCell(buttonCancel)
                        )
                    ) { Spacing = Utilities.Spacing3 }
                )
            ) { Spacing = Utilities.Spacing3 };
        }
    }
}
