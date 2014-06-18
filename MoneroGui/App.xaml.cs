using System.Globalization;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        App()
        {
#if DEBUG
            var cultureInfo = new CultureInfo("en");
#else
            var languageCode = SettingsManager.Appearance.LanguageCode;
            var cultureInfo = languageCode == Helper.DefaultLanguageCode ? CultureInfo.CurrentCulture : new CultureInfo(languageCode);
#endif

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
