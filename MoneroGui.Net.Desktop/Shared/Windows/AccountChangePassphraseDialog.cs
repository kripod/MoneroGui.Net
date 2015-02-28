using Eto.Drawing;
using Eto.Forms;

namespace Jojatekok.MoneroGUI.Forms
{
	public sealed class AccountChangePassphraseDialog : Dialog<string>
	{
        bool IsCurrentPassphraseRequired { get; set; }

        public AccountChangePassphraseDialog(bool isCurrentPassphraseRequired)
        {
            this.SetWindowProperties(
                MoneroGUI.Properties.Resources.AccountUnlockWindowTitle,
                new Size(300, 0)
            );

            IsCurrentPassphraseRequired = isCurrentPassphraseRequired;

            RenderContent();
		}

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            var passwordBoxCurrentPassphrase = new PasswordBox();
            var passwordBoxNewPassphrase1 = new PasswordBox();
            var passwordBoxNewPassphrase2 = new PasswordBox();

            var labelNewPassphrase1 = new Label();
            var labelNewPassphrase2 = new Label();

            var tableLayoutInputFields = new TableLayout {
                Padding = new Padding(Utilities.Padding3),
                Spacing = Utilities.Spacing3
            };

            string baseNewPassphraseText;
            if (IsCurrentPassphraseRequired) {
                baseNewPassphraseText = MoneroGUI.Properties.Resources.AccountChangePassphraseWindowNewPassphrase;
                Title = MoneroGUI.Properties.Resources.AccountChangePassphraseWindowTitleChangePassphrase;
                tableLayoutInputFields.Rows.Add(new TableRow(
                    new Label { Text = MoneroGUI.Properties.Resources.AccountChangePassphraseWindowCurrentPassphrase },
                    passwordBoxCurrentPassphrase
                ));

            } else {
                baseNewPassphraseText = MoneroGUI.Properties.Resources.AccountChangePassphraseWindowPassphrase;
                Title = MoneroGUI.Properties.Resources.AccountChangePassphraseWindowTitleEncryptAccount;
            }

            tableLayoutInputFields.Rows.Add(new TableRow(
                labelNewPassphrase1,
                passwordBoxNewPassphrase1
            ));
            tableLayoutInputFields.Rows.Add(new TableRow(
                labelNewPassphrase2,
                passwordBoxNewPassphrase2
            ));

            labelNewPassphrase1.Text = baseNewPassphraseText + MoneroGUI.Properties.Resources.PunctuationColon;
            labelNewPassphrase2.Text = string.Format(
                Utilities.InvariantCulture,
                "{0} ({1}){2}",
                baseNewPassphraseText,
                MoneroGUI.Properties.Resources.AccountChangePassphraseWindowAgain,
                MoneroGUI.Properties.Resources.PunctuationColon
            );

            var buttonOk = new Button { Text = MoneroGUI.Properties.Resources.TextOk };
            buttonOk.Click += delegate {
                // TODO: Check for the current passphrase's validity if necessary, and display the following text on error:
                // The passphrase entered for the account's decryption is incorrect.

                var newPassphrase = passwordBoxNewPassphrase1.Text;
                if (newPassphrase != passwordBoxNewPassphrase2.Text) {
                    MessageBox.Show(
                        this,
                        MoneroGUI.Properties.Resources.AccountChangePassphraseWindowPassphrasesDoNotMatch,
                        MessageBoxType.Error
                    );
                    return;
                }

                Close(newPassphrase);
            };
            DefaultButton = buttonOk;

            var buttonCancel = new Button { Text = MoneroGUI.Properties.Resources.TextCancel };
            AbortButton = buttonCancel;

            Content = new TableLayout(
                new Label {
                    Text = MoneroGUI.Properties.Resources.AccountChangePassphraseWindowInstruction1
                },
                new Label {
                    Text = MoneroGUI.Properties.Resources.AccountChangePassphraseWindowInstruction2,
                    Font = new Font(Utilities.DefaultFontFamily, Utilities.DefaultFontSize, FontStyle.Bold | FontStyle.Italic)
                },

                tableLayoutInputFields,

                new TableRow(
                    new TableLayout(
                        new TableRow(
                            new TableCell { ScaleWidth = true },
                            new TableCell(buttonOk),
                            new TableCell(buttonCancel)
                        )
                    ) { Spacing = Utilities.Spacing3 }
                )
            );
        }
	}
}
