using System;
using System.Configuration;
using System.Reflection;
using ApiPaths = Jojatekok.MoneroAPI.Paths;

namespace Jojatekok.MoneroGUI
{
    static class SettingsManager
    {
        private const string RelativePathFileUserConfiguration = "user.config";

        private static Configuration Configuration { get; set; }

        public static ConfigSectionGeneral General { get; private set; }
        public static ConfigSectionPaths Paths { get; private set; }

        static SettingsManager()
        {
            // Directory: %LocalAppData%\[Company]\[AssemblyName]\
            var configurationPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" +
                                    ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company + "\\" +
                                    Assembly.GetExecutingAssembly().GetName().Name + "\\" +
                                    RelativePathFileUserConfiguration;

            var configurationFileMap = new ExeConfigurationFileMap {
                ExeConfigFilename = configurationPath,
                LocalUserConfigFilename = configurationPath,
                RoamingUserConfigFilename = configurationPath,
            };

            Configuration = ConfigurationManager.OpenMappedExeConfiguration(configurationFileMap, ConfigurationUserLevel.PerUserRoamingAndLocal);

            // Don't include unnecessary information within ConfigurationSections' type declarations
            Configuration.TypeStringTransformer = delegate(string input) {
                var inputSplit = input.Split(',');
                return inputSplit[0] + "," + inputSplit[1];
            };

            var isSaveRequired = false;

            General = Configuration.GetSection("general") as ConfigSectionGeneral;
            if (General == null) {
                isSaveRequired = true;
                General = new ConfigSectionGeneral();
                Configuration.Sections.Add("general", General);
            }

            Paths = Configuration.GetSection("paths") as ConfigSectionPaths;
            if (Paths == null) {
                isSaveRequired = true;
                Paths = new ConfigSectionPaths();
                Configuration.Sections.Add("paths", Paths);
            }

            if (isSaveRequired) Configuration.Save(ConfigurationSaveMode.Minimal);
        }

        //private static bool UpgradeIfNecessary()
        //{
        //    // <-- Get the base directory of different versions' configurations -->

        //    var baseConfigDirectory = Configuration.FilePath;
        //    var currentConfigDirectory = baseConfigDirectory.Substring(0, baseConfigDirectory.LastIndexOf('\\'));
        //    baseConfigDirectory = currentConfigDirectory.Substring(0, currentConfigDirectory.LastIndexOf('\\'));

        //    // <-- Obtain the highest configuration file available -->

        //    if (!Directory.Exists(baseConfigDirectory)) return false;
        //    var configDirectories = Directory.GetDirectories(baseConfigDirectory);
        //    if (configDirectories.Length == 0) return false;

        //    Version highestVersion = null;
        //    for (var i = configDirectories.Length - 1; i >= 0; i--) {
        //        var directory = configDirectories[i];
        //        if (!File.Exists(directory + "\\" + RelativePathFileUserConfiguration)) continue;

        //        var version = new Version(directory.Substring(directory.LastIndexOf('\\') + 1));
        //        if (version < Helper.ApplicationVersion) {
        //            if (highestVersion == null || version > highestVersion) {
        //                highestVersion = version;
        //            }
        //        }
        //    }

        //    // <-- Move the latest configuration to the current one's path -->

        //    if (highestVersion != null) {
        //        var latestConfigPath = baseConfigDirectory + "\\" + highestVersion + "\\" + RelativePathFileUserConfiguration;
        //        var currentConfigPath = Configuration.FilePath;

        //        if (!Directory.Exists(currentConfigDirectory)) Directory.CreateDirectory(currentConfigDirectory);
        //        File.Move(latestConfigPath, currentConfigPath);

        //        // Re-initialize Configuration
        //        Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

        //    } else {
        //        return false;
        //    }

        //    // <-- Delete old configurations -->

        //    var oldConfigDirectories = new List<string>(configDirectories);
        //    oldConfigDirectories.Remove(currentConfigDirectory);

        //    for (var i = oldConfigDirectories.Count - 1; i >= 0; i--) {
        //        Directory.Delete(oldConfigDirectories[i], true);
        //    }

        //    return true;
        //}

        public class ConfigSectionGeneral : ConfigurationSection
        {
            public ConfigSectionGeneral()
            {
                SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            }

            [ConfigurationProperty("languageCode", DefaultValue = Helper.DefaultLanguageCode)]
            public string LanguageCode {
                get { return base["languageCode"] as string; }
                set {
                    base["languageCode"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }

            [ConfigurationProperty("walletDefaultPassword", DefaultValue = "x")]
            public string WalletDefaultPassword {
                get { return base["walletDefaultPassword"] as string; }
                set {
                    base["walletDefaultPassword"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }

            [ConfigurationProperty("transactionsDefaultMixCount", DefaultValue = 0)]
            [IntegerValidator(MinValue = 0)]
            public int TransactionsDefaultMixCount {
                get { return (int)base["transactionsDefaultMixCount"]; }
                set {
                    base["transactionsDefaultMixCount"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }
        }

        public class ConfigSectionPaths : ConfigurationSection
        {
            public ConfigSectionPaths()
            {
                SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            }

            [ConfigurationProperty("directoryWalletBackups", DefaultValue = ApiPaths.DefaultDirectoryWalletBackups)]
            public string DirectoryWalletBackups {
                get { return base["directoryWalletBackups"] as string; }
                set {
                    base["directoryWalletBackups"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }

            [ConfigurationProperty("fileWalletData", DefaultValue = ApiPaths.DefaultFileWalletData)]
            public string FileWalletData {
                get { return base["fileWalletData"] as string; }
                set {
                    base["fileWalletData"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }

            [ConfigurationProperty("softwareDaemon", DefaultValue = ApiPaths.DefaultSoftwareDaemon)]
            public string SoftwareDaemon {
                get { return base["softwareDaemon"] as string; }
                set {
                    base["softwareDaemon"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }

            [ConfigurationProperty("softwareWallet", DefaultValue = ApiPaths.DefaultSoftwareWallet)]
            public string SoftwareWallet {
                get { return base["softwareWallet"] as string; }
                set {
                    base["softwareWallet"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }

            [ConfigurationProperty("softwareMiner", DefaultValue = ApiPaths.DefaultSoftwareMiner)]
            public string SoftwareMiner {
                get { return base["softwareMiner"] as string; }
                set {
                    base["softwareMiner"] = value;
                    CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
                }
            }
        }
    }
}
