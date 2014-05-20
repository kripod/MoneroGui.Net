using System;

namespace MoneroClient
{
    static class Paths
    {
        static Paths()
        {
            ConfigBasePath = AppDomain.CurrentDomain.BaseDirectory;
            ConfigRelativePathResources = Environment.Is64BitOperatingSystem ?
                                          @"Resources\64-bit\" :
                                          @"Resources\32-bit\";
        }

        private static string ConfigBasePath { get; set; }

        private const string ConfigRelativePathWalletData = @"WalletData\";
        private const string RelativePathFileWalletData = "wallet.bin";
        private const string RelativePathFileWalletAddress = RelativePathFileWalletData + ".address.txt";

        private static string ConfigRelativePathResources { get; set; }
        private const string RelativePathResourceDaemon = "bitmonerod.exe";
        private const string RelativePathResourceWallet = "simplewallet.exe";
        private const string RelativePathResourceMiner = "simpleminer.exe";

        internal static string DirectoryWalletData { get { return ConfigBasePath + ConfigRelativePathWalletData; } }
        internal static string FileWalletData { get { return ConfigBasePath + ConfigRelativePathWalletData + RelativePathFileWalletData; } }
        internal static string FileWalletAddress { get { return ConfigBasePath + ConfigRelativePathWalletData + RelativePathFileWalletAddress; } }

        internal static string ResourceDaemon { get { return ConfigBasePath + ConfigRelativePathResources + RelativePathResourceDaemon; } }
        internal static string ResourceWallet { get { return ConfigBasePath + ConfigRelativePathResources + RelativePathResourceWallet; } }
        internal static string ResourceMiner { get { return ConfigBasePath + ConfigRelativePathResources + RelativePathResourceMiner; } }
    }
}
