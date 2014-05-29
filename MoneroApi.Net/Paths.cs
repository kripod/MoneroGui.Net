using System;

namespace Jojatekok.MoneroAPI
{
    public class Paths
    {
        private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        private const string DefaultRelativePathDirectoryWalletData = @"WalletData\";
        private const string DefaultRelativePathDirectoryResources = @"Resources\";
        private static readonly string DefaultRelativePathDirectorySoftwares = DefaultRelativePathDirectoryResources + (
            Environment.Is64BitOperatingSystem ?
            @"64-bit\" :
            @"32-bit\"
        );

        private string _directoryWalletBackups = BasePath + DefaultRelativePathDirectoryWalletData + @"Backups\";
        public string DirectoryWalletBackups {
            get { return _directoryWalletBackups; }
            set { _directoryWalletBackups = value; }
        }

        private string _fileWalletData = BasePath + DefaultRelativePathDirectoryWalletData + "wallet.bin";
        public string FileWalletData {
            get { return _fileWalletData; }
            set { _fileWalletData = value; }
        }

        public string FileWalletAddress {
            get { return FileWalletData + ".address.txt"; }
        }

        private string _softwareDaemon = BasePath + DefaultRelativePathDirectorySoftwares + "bitmonerod.exe";
        public string SoftwareDaemon {
            get { return _softwareDaemon; }
            set { _softwareDaemon = value; }
        }

        private string _softwareWallet = BasePath + DefaultRelativePathDirectorySoftwares + "simplewallet.exe";
        public string SoftwareWallet {
            get { return _softwareWallet; }
            set { _softwareWallet = value; }
        }

        private string _softwareMiner = BasePath + DefaultRelativePathDirectorySoftwares + "simpleminer.exe";
        public string SoftwareMiner {
            get { return _softwareMiner; }
            set { _softwareMiner = value; }
        }
    }
}
