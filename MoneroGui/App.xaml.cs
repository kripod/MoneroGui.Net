using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        App()
        {
#if !DEBUG
            // Log unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

            var languageCode = SettingsManager.Appearance.LanguageCode;
            var languageCulture = languageCode == StaticObjects.DefaultLanguageCode ? Helper.DefaultUiCulture : new CultureInfo(languageCode);

            CultureManager.CurrentCulture = languageCulture;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Debug.Assert(exception != null, "exception != null");

            using (var stream = new StreamWriter(StaticObjects.ApplicationBaseDirectory + "CrashLogs.txt", true)) {
                stream.WriteLine(Helper.NewLineString +
                                 exception.Message + Helper.NewLineString +
                                 exception.StackTrace);
            }
        }
    }
}
