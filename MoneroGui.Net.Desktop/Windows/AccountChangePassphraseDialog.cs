using Eto.Drawing;
using Eto.Forms;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class AccountChangePassphraseDialog : Dialog<string>
    {
        bool IsCurrentPassphraseRequired { get; set; }

        public AccountChangePassphraseDialog(bool isCurrentPassphraseRequired)
        {
            this.SetWindowProperties(
                Desktop.Properties.Resources.AccountUnlockWindowTitle,
                new Size(0, 0)
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
                Padding = new Padding(0, Utilities.Padding3),
                Spacing = Utilities.Spacing3
            };

            string baseNewPassphraseText;
            if (IsCurrentPassphraseRequired) {
                baseNewPassphraseText = Desktop.Properties.Resources.AccountChangePassphraseWindowNewPassphrase;
                Title = Desktop.Properties.Resources.AccountChangePassphraseWindowTitleChangePassphrase;
                tableLayoutInputFields.Rows.Add(new TableRow(
                    new Label { Text = Desktop.Properties.Resources.AccountChangePassphraseWindowCurrentPassphrase },
                    passwordBoxCurrentPassphrase
                ));

            } else {
                baseNewPassphraseText = Desktop.Properties.Resources.AccountChangePassphraseWindowPassphrase;
                Title = Desktop.Properties.Resources.AccountChangePassphraseWindowTitleEncryptAccount;
            }

            tableLayoutInputFields.Rows.Add(new TableRow(
                labelNewPassphrase1,
                passwordBoxNewPassphrase1
            ));
            tableLayoutInputFields.Rows.Add(new TableRow(
                labelNewPassphrase2,
                passwordBoxNewPassphrase2
            ));

            labelNewPassphrase1.Text = baseNewPassphraseText + Desktop.Properties.Resources.PunctuationColon;
            labelNewPassphrase2.Text = string.Format(
                Utilities.InvariantCulture,
                "{0} ({1}){2}",
                baseNewPassphraseText,
                Desktop.Properties.Resources.AccountChangePassphraseWindowAgain,
                Desktop.Properties.Resources.PunctuationColon
            );

            var buttonOk = new Button { Text = Desktop.Properties.Resources.TextOk };
            buttonOk.Click += delegate {
                // TODO: Check for the current passphrase's validity if necessary, and display the following text on error:
                // The passphrase entered for the account's decryption is incorrect.

                var newPassphrase = passwordBoxNewPassphrase1.Text;
                if (newPassphrase != passwordBoxNewPassphrase2.Text) {
                    this.ShowError(Desktop.Properties.Resources.AccountChangePassphraseWindowPassphrasesDoNotMatch);
                    return;
                }

                Close(newPassphrase);
            };
            DefaultButton = buttonOk;

            var buttonCancel = new Button { Text = Desktop.Properties.Resources.TextCancel };
            AbortButton = buttonCancel;
            AbortButton.Click += delegate { Close(null); };

            Content = new TableLayout(
                new Label {
                    Text = Desktop.Properties.Resources.AccountChangePassphraseWindowInstruction1
                },
                new Label {
                    Text = Desktop.Properties.Resources.AccountChangePassphraseWindowInstruction2,
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
