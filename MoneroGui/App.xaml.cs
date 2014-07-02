using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        private static readonly string MutexName = "Local\\" +
                                                   Helper.GetAssemblyAttribute<AssemblyTitleAttribute>().Title +
                                                   " {" + Helper.GetAssemblyAttribute<GuidAttribute>().Value + "}";
        private static readonly Mutex MutexObject = new Mutex(true, MutexName);

        App()
        {
            var applicationFirstInstanceActivatorMessage = NativeMethods.RegisterWindowMessage(MutexName);
            StaticObjects.ApplicationFirstInstanceActivatorMessage = applicationFirstInstanceActivatorMessage;

            if (MutexObject.WaitOne(0, true)) {
                // This is the first instance of the application
                MutexObject.ReleaseMutex();

            } else {
                // Notify the first instance to let it be shown
                NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, applicationFirstInstanceActivatorMessage, IntPtr.Zero, IntPtr.Zero);
                
                var currentProcess = Process.GetCurrentProcess();
                var processesWithMatchingName = Process.GetProcessesByName(currentProcess.ProcessName);

                // Try to set the first instance's MainWindow active
                for (var i = processesWithMatchingName.Length - 1; i >= 0; i--) {
                    var process = processesWithMatchingName[i];
                    if (process.Id != currentProcess.Id) {
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }

                Shutdown();
                return;
            }

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
            if (!StaticObjects.IsUnhandledExceptionLoggingEnabled) return;

            var exception = e.ExceptionObject as Exception;
            Debug.Assert(exception != null, "exception != null");

            using (var stream = new StreamWriter(StaticObjects.ApplicationBaseDirectory + "CrashLogs.txt", true)) {
                stream.WriteLine(
                    Helper.NewLineString +
                    exception.Message + Helper.NewLineString +
                    exception.StackTrace
                );
            }
        }
    }
}
