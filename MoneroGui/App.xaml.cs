using System.Globalization;
using System.Threading;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        App()
        {
            // TODO: Initialize the desired language from a configuration file
            var cultureInfo = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            MoneroGUI.Properties.Resources.Culture = cultureInfo;
        }
    }
}
