using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ApiPathSettings = Jojatekok.MoneroAPI.Settings.PathSettings;

namespace Jojatekok.MoneroGUI
{
    public static class SettingsManager
    {
        private const ulong SettingsVersionLatest = 2;
        private const string RelativePathFileUserConfiguration = "user.config";

        private const ulong DefaultValueGeneralSectionTransactionsDefaultFee = 5000000000;

        private static Configuration Configuration { get; set; }

        private static bool _isAutoSaveEnabled = true;
        public static bool IsAutoSaveEnabled {
            get { return _isAutoSaveEnabled; }
            set { _isAutoSaveEnabled = value; }
        }

        public static ConfigSectionGeneral General { get; private set; }
        public static ConfigSectionPaths Paths { get; private set; }
        public static ConfigSectionNetwork Network { get; private set; }
        public static ConfigSectionAppearance Appearance { get; private set; }
        public static ConfigSectionAddressBook AddressBook { get; private set; }

        static SettingsManager()
        {
            InitializeConfiguration();

            // Settings are ready to be used from here
            StaticObjects.Initialize();
        }

        private static void InitializeConfiguration()
        {
            // Directory: %LocalAppData%\[Company]\[AssemblyName]\
            var configurationPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Helper.GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                StaticObjects.ApplicationAssemblyName.Name,
                RelativePathFileUserConfiguration
            );

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

            LoadOrCreateSections();
        }

        private static void LoadOrCreateSections()
        {
            var isSaveRequired = false;
            var isNewFileCreated = false;

            General = Configuration.GetSection("general") as ConfigSectionGeneral;
            if (General == null) {
                isNewFileCreated = true;
                General = new ConfigSectionGeneral();
                Configuration.Sections.Add("general", General);
            }

            Paths = Configuration.GetSection("paths") as ConfigSectionPaths;
            if (Paths == null) {
                isSaveRequired = true;
                Paths = new ConfigSectionPaths();
                Configuration.Sections.Add("paths", Paths);
            }

            Network = Configuration.GetSection("network") as ConfigSectionNetwork;
            if (Network == null) {
                isSaveRequired = true;
                Network = new ConfigSectionNetwork();
                Configuration.Sections.Add("network", Network);
            }

            Appearance = Configuration.GetSection("appearance") as ConfigSectionAppearance;
            if (Appearance == null) {
                isSaveRequired = true;
                Appearance = new ConfigSectionAppearance();
                Configuration.Sections.Add("appearance", Appearance);
            }

            AddressBook = Configuration.GetSection("addressBook") as ConfigSectionAddressBook;
            if (AddressBook == null) {
                isSaveRequired = true;
                AddressBook = new ConfigSectionAddressBook();
                Configuration.Sections.Add("addressBook", AddressBook);
            }

            var metaData = Configuration.GetSection("metaData") as ConfigSectionMetaData;
            if (metaData == null) {
                isSaveRequired = true;
                metaData = new ConfigSectionMetaData();
                Configuration.Sections.Add("metaData", metaData);
            }

            // Don't try to upgrade the settings whether a new configuration file was created
            if (isNewFileCreated) {
                metaData.SettingsVersion = SettingsVersionLatest;
                SaveSettings();
                return;
            }

            // Upgrade settings if necessary
            var settingsVersionOld = metaData.SettingsVersion;
            if (settingsVersionOld < SettingsVersionLatest) {
                UpgradeFromVersion(settingsVersionOld);
                metaData.SettingsVersion = SettingsVersionLatest;
                SaveSettings();
                return;
            }

            // Save settings if necessary
            if (isSaveRequired) {
                SaveSettings();
            }
        }

        private static void UpgradeFromVersion(ulong oldConfigurationVersion)
        {
            IsAutoSaveEnabled = false;

            if (oldConfigurationVersion < 2) {
                if (General.TransactionsDefaultFee < DefaultValueGeneralSectionTransactionsDefaultFee) {
                    General.TransactionsDefaultFee = DefaultValueGeneralSectionTransactionsDefaultFee;
                }
            } else {
                IsAutoSaveEnabled = true;
                return;
            }

            if (oldConfigurationVersion < 1) {
                General.TransactionsDefaultFee = DefaultValueGeneralSectionTransactionsDefaultFee;
            } else {
                IsAutoSaveEnabled = true;
                return;
            }

            IsAutoSaveEnabled = true;
        }

        public static void SaveSettings()
        {
            Configuration.Save(ConfigurationSaveMode.Modified);
            InitializeConfiguration();
        }

        private static void AutoSaveSettings()
        {
            if (IsAutoSaveEnabled) SaveSettings();
        }

        private static void SetDefaultSectionInformation(this ConfigurationSection configSection)
        {
            configSection.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
        }

        public class ConfigSectionGeneral : ConfigurationSection
        {
            public ConfigSectionGeneral()
            {
                this.SetDefaultSectionInformation();
            }

            [ConfigurationProperty("isStartableOnSystemLogin", DefaultValue = false)]
            public bool IsStartableOnSystemLogin {
                get { return (bool)base["isStartableOnSystemLogin"]; }
                set {
                    base["isStartableOnSystemLogin"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isSafeShutdownEnabled", DefaultValue = true)]
            public bool IsSafeShutdownEnabled {
                get { return (bool)base["isSafeShutdownEnabled"]; }
                set {
                    base["isSafeShutdownEnabled"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isRegularWalletBackupEnabled", DefaultValue = false)]
            public bool IsRegularWalletBackupEnabled {
                get { return (bool)base["isRegularWalletBackupEnabled"]; }
                set {
                    base["isRegularWalletBackupEnabled"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("transactionsDefaultMixCount", DefaultValue = (ulong)0)]
            public ulong TransactionsDefaultMixCount {
                get { return (ulong)base["transactionsDefaultMixCount"]; }
                set {
                    base["transactionsDefaultMixCount"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("transactionsDefaultFee", DefaultValue = DefaultValueGeneralSectionTransactionsDefaultFee)]
            public ulong TransactionsDefaultFee {
                get { return (ulong)base["transactionsDefaultFee"]; }
                set {
                    base["transactionsDefaultFee"] = value;
                    AutoSaveSettings();
                }
            }
        }

        public class ConfigSectionPaths : ConfigurationSection
        {
            public ConfigSectionPaths()
            {
                this.SetDefaultSectionInformation();
            }

            [ConfigurationProperty("directoryDaemonData", DefaultValue = null)]
            public string DirectoryDaemonData {
                get {
                    var output = base["directoryDaemonData"] as string;
                    return !string.IsNullOrEmpty(output) ? output : ApiPathSettings.DefaultDirectoryDaemonData;
                }

                set {
                    if (value == ApiPathSettings.DefaultDirectoryDaemonData) value = null;
                    base["directoryDaemonData"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("directoryWalletBackups", DefaultValue = ApiPathSettings.DefaultDirectoryWalletBackups)]
            public string DirectoryWalletBackups {
                get { return base["directoryWalletBackups"] as string; }
                set {
                    base["directoryWalletBackups"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("fileWalletData", DefaultValue = ApiPathSettings.DefaultFileWalletData)]
            public string FileWalletData {
                get { return base["fileWalletData"] as string; }
                set {
                    base["fileWalletData"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareDaemon", DefaultValue = ApiPathSettings.DefaultSoftwareDaemon)]
            public string SoftwareDaemon {
                get { return base["softwareDaemon"] as string; }
                set {
                    base["softwareDaemon"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareWallet", DefaultValue = ApiPathSettings.DefaultSoftwareWallet)]
            public string SoftwareWallet {
                get { return base["softwareWallet"] as string; }
                set {
                    base["softwareWallet"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareMiner", DefaultValue = ApiPathSettings.DefaultSoftwareMiner)]
            public string SoftwareMiner {
                get { return base["softwareMiner"] as string; }
                set {
                    base["softwareMiner"] = value;
                    AutoSaveSettings();
                }
            }
        }

        public class ConfigSectionNetwork : ConfigurationSection
        {
            public ConfigSectionNetwork()
            {
                this.SetDefaultSectionInformation();
            }

            [ConfigurationProperty("rpcUrlHost", DefaultValue = "localhost")]
            public string RpcUrlHost {
                get { return base["rpcUrlHost"] as string; }
                set {
                    base["rpcUrlHost"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("rpcUrlPortDaemon", DefaultValue = (ushort)18081)]
            public ushort RpcUrlPortDaemon {
                get { return (ushort)base["rpcUrlPortDaemon"]; }
                set {
                    base["rpcUrlPortDaemon"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("rpcUrlPortWallet", DefaultValue = (ushort)18082)]
            public ushort RpcUrlPortWallet {
                get { return (ushort)base["rpcUrlPortWallet"]; }
                set {
                    base["rpcUrlPortWallet"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isProxyEnabled", DefaultValue = false)]
            public bool IsProxyEnabled {
                get { return (bool)base["isProxyEnabled"]; }
                set {
                    base["isProxyEnabled"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("proxyHost", DefaultValue = null)]
            public string ProxyHost {
                get { return base["proxyHost"] as string; }
                set {
                    base["proxyHost"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("proxyPort", DefaultValue = null)]
            public ushort? ProxyPort {
                get { return base["proxyPort"] as ushort?; }
                set {
                    base["proxyPort"] = value;
                    AutoSaveSettings();
                }
            }
        }

        public class ConfigSectionAppearance : ConfigurationSection
        {
            public ConfigSectionAppearance()
            {
                this.SetDefaultSectionInformation();
            }

            [ConfigurationProperty("languageCode", DefaultValue = StaticObjects.DefaultLanguageCode)]
            public string LanguageCode {
                get { return base["languageCode"] as string; }
                set {
                    base["languageCode"] = value;
                    AutoSaveSettings();
                }
            }
        }

        public class ConfigSectionAddressBook : ConfigurationSection
        {
            public ConfigSectionAddressBook()
            {
                this.SetDefaultSectionInformation();
            }

            [ConfigurationProperty("elements", IsDefaultCollection = true)]
            [ConfigurationCollection(typeof(ConfigElementCollectionContact))]
            public ConfigElementCollectionContact Elements {
                get { return (ConfigElementCollectionContact)base["elements"]; }
            }
        }

        private class ConfigSectionMetaData : ConfigurationSection
        {
            public ConfigSectionMetaData()
            {
                this.SetDefaultSectionInformation();
            }

            [ConfigurationProperty("settingsVersion", DefaultValue = (ulong)0)]
            public ulong SettingsVersion {
                get { return (ulong)base["settingsVersion"]; }
                set { base["settingsVersion"] = value; }
            }
        }

        public class ConfigElementContact : ConfigurationElement
        {
            [ConfigurationProperty("label", IsRequired = true)]
            public string Label {
                get { return base["label"] as string; }
                set { base["label"] = value; }
            }

            [ConfigurationProperty("address", IsRequired = true)]
            public string Address {
                get { return base["address"] as string; }
                set { base["address"] = value; }
            }

            public ConfigElementContact(string label, string address)
            {
                Label = label;
                Address = address;
            }
        }

        public class ConfigElementCollectionContact : ConfigurationElementCollection, IEnumerable<ConfigElementContact>
        {
            public override ConfigurationElementCollectionType CollectionType {
                get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
            }

            protected override sealed ConfigurationElement CreateNewElement()
            {
                return new ConfigElementContact(string.Empty, string.Empty);
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                Debug.Assert(element as ConfigElementContact != null, "element as ConfigElementContact != null");
                return (element as ConfigElementContact).Label;
            }

            public void Add(ConfigElementContact element)
            {
                BaseAdd(element, false);
                AutoSaveSettings();
            }

            public void Remove(ConfigElementContact element)
            {
                if (BaseIndexOf(element) >= 0) {
                    BaseRemove(element.Label);
                    AutoSaveSettings();
                }
            }

            public void Clear()
            {
                BaseClear();
                AutoSaveSettings();
            }

            public ConfigElementContact this[int index] {
                get { return BaseGet(index) as ConfigElementContact; }

                set {
                    IsAutoSaveEnabled = false;

                    if (BaseGet(index) != null) BaseRemoveAt(index);
                    BaseAdd(index, value);

                    IsAutoSaveEnabled = true;
                    SaveSettings();
                }
            }

            public new IEnumerator<ConfigElementContact> GetEnumerator()
            {
                for (var i = Count - 1; i >= 0; i--) {
                    yield return this[i];
                }
            }
        }
    }
}
