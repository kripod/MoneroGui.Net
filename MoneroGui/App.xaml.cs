using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jojatekok.MoneroGUI
{
    public partial class App
    {
        private static readonly string MutexName = "Global\\" +
                                                   Helper.GetAssemblyAttribute<AssemblyTitleAttribute>().Title +
                                                   " {" + Helper.GetAssemblyAttribute<GuidAttribute>().Value + "}";
        private static Mutex MutexObject { get; set; }

        App()
        {
#if !DEBUG
            var applicationFirstInstanceActivatorMessage = NativeMethods.RegisterWindowMessage(MutexName);
            var createdNewMutex = true;

            MutexObject = new Mutex(true, MutexName, out createdNewMutex);

            if (createdNewMutex) {
                // This is the first instance of the application
                MutexObject.ReleaseMutex();
                StaticObjects.ApplicationFirstInstanceActivatorMessage = applicationFirstInstanceActivatorMessage;

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

        private void StyleTextBoxTransparent_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            Debug.Assert(textBox != null, "textBox != null");

            textBox.SelectAll();
            e.Handled = true;
        }

        private void StyleTextBoxTransparent_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            Debug.Assert(textBox != null, "textBox != null");

            textBox.SelectionLength = 0;
            e.Handled = true;
        }
    }
}
