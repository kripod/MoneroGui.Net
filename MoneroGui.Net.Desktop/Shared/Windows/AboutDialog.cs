using Eto.Drawing;
using Eto.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI.Forms
{
	public sealed class AboutDialog : Dialog
	{
	    private readonly TextArea _textAreaLicense = new TextArea { ReadOnly = true };
        private readonly Button _buttonShowThirdPartyLicenses = new Button { Text = MoneroGUI.Properties.Resources.AboutWindowThirdPartyLicenses };

	    private TextArea TextAreaLicense {
	        get { return _textAreaLicense; }
	    }

        private Button ButtonShowThirdPartyLicenses {
            get { return _buttonShowThirdPartyLicenses; }
        }

        private static string LicenseText { get; set; }

	    public AboutDialog()
		{
		    this.SetWindowProperties(
                MoneroGUI.Properties.Resources.AboutWindowTitle,
                new Size(600, 300)
		    );

            RenderContent();

            if (LicenseText == null) {
                Task.Factory.StartNew(LoadLicenseText);
            } else {
                TextAreaLicense.Text = LicenseText;
            }

            CheckThirdPartyLicensesAvailability();
		}

        private void RenderContent()
        {
            var linkButtonCreditIcons = new LinkButton { Text = "VisualPharm" };
            linkButtonCreditIcons.Click += delegate { Process.Start("http://www.visualpharm.com"); };

            ButtonShowThirdPartyLicenses.Click += delegate {
                if (CheckThirdPartyLicensesAvailability()) {
                    Process.Start(Utilities.PathDirectoryThirdPartyLicenses);
                }
            };

            var dynamicLayoutMain = new DynamicLayout { DefaultPadding = new Padding(Utilities.Padding4) };

            dynamicLayoutMain.BeginHorizontal();
            dynamicLayoutMain.BeginVertical();
            dynamicLayoutMain.AddCentered(new ImageView { Image = Utilities.LoadImage("Icon-192x192") });
            dynamicLayoutMain.EndVertical();

            dynamicLayoutMain.BeginVertical();
            dynamicLayoutMain.Add(new TableLayout(
                new TableRow(
                    new Label {
                        Text = MoneroGUI.Properties.Resources.TextClientName,
                        HorizontalAlign = HorizontalAlign.Center,
                        Font = new Font(SystemFont.Default, Utilities.FontSize3)
                    }
                ),

                new TableRow(
                    new Label {
                        Text = "v" + Utilities.ApplicationVersionString,
                        HorizontalAlign = HorizontalAlign.Center
                    }
                ),

                new TableRow(
                    new TableLayout(
                        TextAreaLicense
                    ) { Padding = new Padding(0, Utilities.Padding3) }
                ) { ScaleHeight = true },

                new TableRow(
                    new TableRow(
                        new TableCell(
                            new TableLayout(
                                new TableRow(
                                    new TableCell(new Label { Text = MoneroGUI.Properties.Resources.AboutWindowCreditIcons + " " }, true)
                                ),
                                new TableRow(
                                    new TableCell(linkButtonCreditIcons, true)
                                )
                            ),
                            true
                        ),

                        new TableCell(ButtonShowThirdPartyLicenses)
                    )
                )
            ));
            dynamicLayoutMain.EndVertical();
            dynamicLayoutMain.EndHorizontal();

            Content = dynamicLayoutMain;
        }

        private void LoadLicenseText()
        {
            if (File.Exists(Utilities.PathFileLicense)) {
                using (var stream = new StreamReader(Utilities.PathFileLicense)) {
                    LicenseText = stream.ReadToEnd();
                    LicenseText = LicenseText.ReWrap();
                }
                Utilities.SyncContextMain.Post(sender => TextAreaLicense.Text = LicenseText, null);

            } else {
                Utilities.SyncContextMain.Post(sender => TextAreaLicense.Text = MoneroGUI.Properties.Resources.AboutWindowLicenseFileNotFound, null);
            }
        }

        private bool CheckThirdPartyLicensesAvailability()
        {
            var output = Directory.Exists(Utilities.PathDirectoryThirdPartyLicenses);
            ButtonShowThirdPartyLicenses.Enabled = output;
            return output;
        }
	}
}
