using System;

namespace Jojatekok.MoneroAPI
{
    public class Paths
    {
        private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        internal const string DirectoryWalletData = @"WalletData\";
        internal const string DirectoryWalletDataBackups = DirectoryWalletData + @"Backups\";
        private string _fileWalletData = DirectoryWalletData + "wallet.bin";

        private const string DirectoryResources = @"Resources\";
        private static readonly string DirectorySoftwares = DirectoryResources + (Environment.Is64BitOperatingSystem ?
                                                                                  @"64-bit\" :
                                                                                  @"32-bit\");
        private string _softwareDaemon = DirectorySoftwares + "bitmonerod.exe";
        private string _softwareWallet = DirectorySoftwares + "simplewallet.exe";
        private string _softwareMiner = DirectorySoftwares + "simpleminer.exe";

        public string FileWalletData {
            get { return BasePath + _fileWalletData; }
            set { _fileWalletData = value; }
        }

        public string FileWalletAddress {
            get { return BasePath + _fileWalletData + ".address.txt"; }
        }

        public string SoftwareDaemon {
            get { return BasePath + _softwareDaemon; }
            set { _softwareDaemon = value; }
        }

        public string SoftwareWallet {
            get { return BasePath + _softwareWallet; }
            set { _softwareWallet = value; }
        }

        public string SoftwareMiner {
            get { return BasePath + _softwareMiner; }
            set { _softwareMiner = value; }
        }
    }
}
