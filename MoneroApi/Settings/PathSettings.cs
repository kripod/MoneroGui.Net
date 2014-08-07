using System;
using System.IO;

namespace Jojatekok.MoneroAPI.Settings
{
    public class PathSettings
    {
        private const string DefaultRelativePathDirectoryWalletData = "WalletData\\";
        private const string DefaultRelativePathDirectorySoftware = "Resources\\Software\\";
        
        public static readonly string DefaultDirectoryDaemonData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bitmonero");
        public const string DefaultDirectoryWalletBackups = DefaultRelativePathDirectoryWalletData + "Backups\\";
        public const string DefaultFileWalletData = DefaultRelativePathDirectoryWalletData + "wallet.bin";
        public const string DefaultSoftwareDaemon = DefaultRelativePathDirectorySoftware + "bitmonerod.exe";
        public const string DefaultSoftwareWallet = DefaultRelativePathDirectorySoftware + "rpcwallet.exe";

        private string _directoryDaemonData = DefaultDirectoryDaemonData;
        public string DirectoryDaemonData {
            get { return Helper.GetAbsolutePath(_directoryDaemonData); }
            set { _directoryDaemonData = value; }
        }

        public string DirectoryWalletData {
            get {
                var lastIndexOfSlash = FileWalletData.LastIndexOf('\\');
                return lastIndexOfSlash >= 0 ? Helper.GetAbsolutePath(FileWalletData.Substring(0, FileWalletData.LastIndexOf('\\'))) : StaticObjects.ApplicationDirectory;
            }
        }

        private string _directoryWalletBackups = DefaultDirectoryWalletBackups;
        public string DirectoryWalletBackups {
            get { return Helper.GetAbsolutePath(_directoryWalletBackups); }
            set { _directoryWalletBackups = value; }
        }

        private string _fileWalletData = DefaultFileWalletData;
        public string FileWalletData {
            get { return Helper.GetAbsolutePath(_fileWalletData); }
            set { _fileWalletData = value; }
        }

        public string FileWalletDataKeys {
            get { return Helper.GetAbsolutePath(FileWalletData + ".keys"); }
        }

        private string _softwareDaemon = DefaultSoftwareDaemon;
        public string SoftwareDaemon {
            get { return Helper.GetAbsolutePath(_softwareDaemon); }
            set { _softwareDaemon = value; }
        }

        private string _softwareWallet = DefaultSoftwareWallet;
        public string SoftwareWallet {
            get { return Helper.GetAbsolutePath(_softwareWallet); }
            set { _softwareWallet = value; }
        }
    }
}
