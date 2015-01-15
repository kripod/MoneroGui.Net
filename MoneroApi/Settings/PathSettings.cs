using System;
using System.IO;

namespace Jojatekok.MoneroAPI.Settings
{
    public class PathSettings
    {
        private const string DefaultRelativePathDirectoryAccountData = "AccountData\\";
        private const string DefaultRelativePathDirectorySoftware = "Resources\\Software\\";
        
        public static readonly string DefaultDirectoryDaemonData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bitmonero");
        public const string DefaultDirectoryAccountBackups = DefaultRelativePathDirectoryAccountData + "Backups\\";
        public const string DefaultFileAccountData = DefaultRelativePathDirectoryAccountData + "account.bin";
        public const string DefaultSoftwareDaemon = DefaultRelativePathDirectorySoftware + "bitmonerod.exe";
        public const string DefaultSoftwareAccountManager = DefaultRelativePathDirectorySoftware + "simplewallet.exe";

        private string _directoryDaemonData = DefaultDirectoryDaemonData;
        public string DirectoryDaemonData {
            get { return Helper.GetAbsolutePath(_directoryDaemonData); }
            set { _directoryDaemonData = value; }
        }

        public string DirectoryAccountData {
            get {
                var lastIndexOfSlash = FileAccountData.LastIndexOf('\\');
                return lastIndexOfSlash >= 0 ? Helper.GetAbsolutePath(FileAccountData.Substring(0, FileAccountData.LastIndexOf('\\'))) : StaticObjects.ApplicationDirectory;
            }
        }

        private string _directoryAccountBackups = DefaultDirectoryAccountBackups;
        public string DirectoryAccountBackups {
            get { return Helper.GetAbsolutePath(_directoryAccountBackups); }
            set { _directoryAccountBackups = value; }
        }

        private string _fileAccountData = DefaultFileAccountData;
        public string FileAccountData {
            get { return Helper.GetAbsolutePath(_fileAccountData); }
            set { _fileAccountData = value; }
        }

        public string FileAccountDataKeys {
            get { return Helper.GetAbsolutePath(FileAccountData + ".keys"); }
        }

        private string _softwareDaemon = DefaultSoftwareDaemon;
        public string SoftwareDaemon {
            get { return Helper.GetAbsolutePath(_softwareDaemon); }
            set { _softwareDaemon = value; }
        }

        private string _softwareAccountManager = DefaultSoftwareAccountManager;
        public string SoftwareAccountManager {
            get { return Helper.GetAbsolutePath(_softwareAccountManager); }
            set { _softwareAccountManager = value; }
        }
    }
}
