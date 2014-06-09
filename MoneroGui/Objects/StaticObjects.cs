using Jojatekok.MoneroAPI;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Media;

namespace Jojatekok.MoneroGUI
{
    static class StaticObjects
    {
        public static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly ImageSource ApplicationIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location).ToImageSource();

        public static MoneroClient MoneroClient { get; private set; }

        public static Logger LoggerDaemon { get; private set; }
        public static Logger LoggerWallet { get; private set; }

        static StaticObjects()
        {
            // TODO: Implement custom wallet password support
            var paths = new Paths {
                DirectoryWalletBackups = SettingsManager.Paths.DirectoryWalletBackups,
                FileWalletData = SettingsManager.Paths.FileWalletData,
                SoftwareDaemon = SettingsManager.Paths.SoftwareDaemon,
                SoftwareWallet = SettingsManager.Paths.SoftwareWallet,
                SoftwareMiner = SettingsManager.Paths.SoftwareMiner,
            };

            MoneroClient = new MoneroClient(paths, SettingsManager.General.WalletDefaultPassword);

            LoggerDaemon = new Logger();
            LoggerWallet = new Logger();
        }
    }
}
