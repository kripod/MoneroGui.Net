using System.Globalization;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        App()
        {
            var languageCode = SettingsManager.Appearance.LanguageCode;
            var cultureInfo = languageCode == Helper.DefaultLanguageCode ? CultureInfo.CurrentCulture : new CultureInfo(languageCode);

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
