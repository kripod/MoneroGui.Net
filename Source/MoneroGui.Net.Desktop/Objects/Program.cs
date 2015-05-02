using Eto;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Windows;
using System;
using System.Diagnostics;
using System.IO;

namespace Jojatekok.MoneroGUI.Desktop
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Catch unhandled exceptions
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += OnApplicationUnhandledException;
#endif

            AddStyles();

            new Application(Platform.Detect).Run(new MainForm());
        }

        static void OnApplicationUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Debug.Assert(exception != null, "exception != null");

            // Write the exception's details to a log file
            using (var stream = new StreamWriter(Path.Combine(Utilities.ApplicationBaseDirectory, "CrashLogs.txt"), true)) {
                stream.WriteLine(
                    Utilities.NewLineString +
                    exception.Message + Utilities.NewLineString +
                    exception.StackTrace
                );
            }

            // Make sure that Monero core applications get closed before exit
            if (Utilities.MoneroRpcManager != null) {
                Utilities.MoneroRpcManager.Dispose();
            }

            if (Utilities.MoneroProcessManager != null) {
                Utilities.MoneroProcessManager.Dispose();
            }

            // Exit with an error code
            Environment.Exit(1);
        }

        static void AddStyles()
        {
            Style.Add<Label>(null, label => {
                label.VerticalAlignment = VerticalAlignment.Center;
            });
        }
    }
}
