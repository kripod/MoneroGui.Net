using Jojatekok.MoneroGUI.Windows;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jojatekok.MoneroGUI
{
    public partial class App : ISingleInstanceApp
    {
        private static readonly string UniqueName = Helper.GetAssemblyAttribute<AssemblyTitleAttribute>().Title +
                                                    " {" + Helper.GetAssemblyAttribute<GuidAttribute>().Value + "}";

        App()
        {
            // Terminate the application if an instance of it is already running
            if (!SingleInstance<App>.InitializeAsFirstInstance(UniqueName)) {
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

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Allow single instance code to perform cleanup operations
            SingleInstance<App>.Cleanup();
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

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            var mainWindow = MainWindow as MainWindow;
            Debug.Assert(mainWindow != null, "mainWindow != null");

            if (mainWindow.Visibility != Visibility.Visible) {
                mainWindow.SetTrayState(true);
            } else {
                mainWindow.RestoreWindowStateFromMinimized();
            }

            // Handle command line arguments of the application's second instance
            if (args.Count > 1) {
                mainWindow.OpenProtocolUri(args[1]);
            }

            return true;
        }
    }
}
