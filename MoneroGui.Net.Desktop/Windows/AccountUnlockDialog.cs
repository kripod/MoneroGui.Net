﻿using Eto.Drawing;
using Eto.Forms;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class AccountUnlockDialog : Dialog<string>
    {
        public AccountUnlockDialog()
        {
            this.SetWindowProperties(
                MoneroGUI.Desktop.Properties.Resources.AccountUnlockWindowTitle,
                new Size(300, 0)
            );

            RenderContent();
        }

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            var passwordBoxResult = new PasswordBox();

            var buttonOk = new Button { Text = MoneroGUI.Desktop.Properties.Resources.TextOk };
            buttonOk.Click += delegate { Close(passwordBoxResult.Text); };
            DefaultButton = buttonOk;

            var buttonCancel = new Button { Text = MoneroGUI.Desktop.Properties.Resources.TextCancel };
            AbortButton = buttonCancel;
            AbortButton.Click += delegate { Close(null); };

            Content = new TableLayout(
                new Label { Text = MoneroGUI.Desktop.Properties.Resources.AccountUnlockWindowInstruction },

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
