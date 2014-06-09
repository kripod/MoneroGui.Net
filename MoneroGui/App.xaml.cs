using System.Globalization;
using System.Threading;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        App()
        {
#if DEBUG
            var cultureInfo = new CultureInfo("en-US");
#else
            var languageCode = SettingsManager.General.LanguageCode;
            var cultureInfo = languageCode == Helper.DefaultLanguageCode ? CultureInfo.CurrentCulture : new CultureInfo(languageCode);
#endif

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            MoneroGUI.Properties.Resources.Culture = cultureInfo;
        }
    }
}
