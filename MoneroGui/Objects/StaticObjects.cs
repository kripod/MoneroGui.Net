using Jojatekok.MoneroAPI;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace Jojatekok.MoneroGUI
{
    static class StaticObjects
    {
        private static bool IsInitialized { get; set; }

        public static readonly Assembly ApplicationAssembly = Assembly.GetExecutingAssembly();
        public static readonly AssemblyName ApplicationAssemblyName = ApplicationAssembly.GetName();

        public static readonly string ApplicationPath = ApplicationAssemblyName.CodeBase;
        public static readonly string ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly ImageSource ApplicationIcon = Icon.ExtractAssociatedIcon(ApplicationAssembly.Location).ToImageSource();
        public static readonly string ApplicationShortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                                                                             Helper.GetAssemblyAttribute<AssemblyTitleAttribute>().Title + ".lnk");

        public static readonly Version ApplicationVersion = ApplicationAssemblyName.Version;
        public static readonly string ApplicationVersionString = ApplicationVersion.ToString(3);

        public static MoneroClient MoneroClient { get; private set; }

        public static Logger LoggerDaemon { get; private set; }
        public static Logger LoggerWallet { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            // TODO: Implement custom wallet password support
            var pathSettings = SettingsManager.Paths;
            var paths = new Paths {
                DirectoryWalletBackups = pathSettings.DirectoryWalletBackups,
                FileWalletData = pathSettings.FileWalletData,
                SoftwareDaemon = pathSettings.SoftwareDaemon,
                SoftwareWallet = pathSettings.SoftwareWallet,
                SoftwareMiner = pathSettings.SoftwareMiner,
            };

            MoneroClient = new MoneroClient(paths, SettingsManager.General.WalletDefaultPassword);

            LoggerDaemon = new Logger();
            LoggerWallet = new Logger();
        }
    }
}
