using System.Globalization;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        App()
        {
            var languageCode = SettingsManager.Appearance.LanguageCode;
            var languageCulture = languageCode == StaticObjects.DefaultLanguageCode ? Helper.DefaultUiCulture : new CultureInfo(languageCode);

            CultureManager.CurrentCulture = languageCulture;
        }
    }
}
