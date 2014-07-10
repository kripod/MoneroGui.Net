using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class AboutWindow
    {
        private static readonly string ThirdPartyLicensesPath = StaticObjects.ApplicationBaseDirectory + "Licenses";

        private static string LicenseText { get; set; }

        private AboutWindow()
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();

            TextBlockVersion.Text = "v" + StaticObjects.ApplicationVersionString;
            CheckThirdPartyLicensesAvailability();

            if (LicenseText == null) {
                Task.Factory.StartNew(LoadLicenseText);
            } else {
                TextBoxLicenseText.Text = LicenseText;
            }

            this.SetDefaultFocusedElement(TextBoxLicenseText);
        }

        public AboutWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private void LoadLicenseText()
        {
            var licenseFiles = Directory.GetFiles(StaticObjects.ApplicationBaseDirectory, "LICENSE*", SearchOption.TopDirectoryOnly);

            if (licenseFiles.Length != 0) {
                using (var stream = new StreamReader(licenseFiles[0])) {
                    LicenseText = stream.ReadToEnd();
                    LicenseText = LicenseText.ReWrap();
                }

                Dispatcher.BeginInvoke(new Action(() => TextBoxLicenseText.Text = LicenseText), DispatcherPriority.DataBind);

            } else {
                Dispatcher.BeginInvoke(new Action(() => TextBoxLicenseText.Text = Properties.Resources.AboutWindowLicenseFileNotFound), DispatcherPriority.DataBind);
            }
        }

        private bool CheckThirdPartyLicensesAvailability()
        {
            var output = Directory.Exists(ThirdPartyLicensesPath);
            ButtonThirdPartyLicenses.IsEnabled = output;
            return output;
        }

        private void HyperlinkIconCreatorWebsite_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);

            this.SetFocusedElement(TextBoxLicenseText);
            e.Handled = true;
        }

        private void ButtonThirdPartyLicenses_Click(object sender, RoutedEventArgs e)
        {
            if (CheckThirdPartyLicensesAvailability()) {
                Process.Start(ThirdPartyLicensesPath);
            }

            this.SetFocusedElement(TextBoxLicenseText);
        }
    }
}
