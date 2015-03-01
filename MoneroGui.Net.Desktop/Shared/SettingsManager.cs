using Jojatekok.MoneroAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Jojatekok.MoneroGUI
{
    public static class SettingsManager
    {
        private const ulong SettingsVersionLatest = 7;
        private const string RelativePathFileUserConfiguration = "user.config";

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

        public static void Initialize()
        {
            // Directory: %LocalAppData% -> [Company] -> [AssemblyName]
            var configurationPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Utilities.GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                Utilities.ApplicationAssemblyName.Name,
                RelativePathFileUserConfiguration
            );

            var configurationFileMap = new ExeConfigurationFileMap {
                ExeConfigFilename = configurationPath,
                LocalUserConfigFilename = configurationPath,
                RoamingUserConfigFilename = configurationPath,
            };

            Configuration = ConfigurationManager.OpenMappedExeConfiguration(configurationFileMap, ConfigurationUserLevel.PerUserRoamingAndLocal);

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

            if (oldConfigurationVersion <= 2) {
                Paths.SoftwareMiner = null;
            }

            if (oldConfigurationVersion <= 4) {
                General.IsRegularAccountBackupEnabled = General.IsRegularWalletBackupEnabled;
                Paths.SoftwareAccountManager = Paths.SoftwareWallet;
                Paths.DirectoryAccountBackups = Paths.DirectoryWalletBackups;
                Paths.FileAccountData = Paths.FileWalletData;
                Network.RpcUrlPortAccountManager = Network.RpcUrlPortWallet;
            }

            if (oldConfigurationVersion <= 6) {
                Paths.SoftwareAccountManager = Paths.SoftwareAccountManager.Replace("rpcwallet.exe", "simplewallet.exe");
                General.TransactionsDefaultFee = 0;
            }

            IsAutoSaveEnabled = true;
        }

        public static void SaveSettings()
        {
            Configuration.Save(ConfigurationSaveMode.Modified);
            Initialize();
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

            [ConfigurationProperty("isUpdateCheckEnabled", DefaultValue = true)]
            public bool IsUpdateCheckEnabled {
                get { return (bool)base["isUpdateCheckEnabled"]; }
                set {
                    base["isUpdateCheckEnabled"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isUpdateCheckForTestBuildsEnabled", DefaultValue = false)]
            public bool IsUpdateCheckForTestBuildsEnabled {
                get { return (bool)base["isUpdateCheckForTestBuildsEnabled"]; }
                set {
                    base["isUpdateCheckForTestBuildsEnabled"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isStartableOnSystemLogin", DefaultValue = false)]
            public bool IsStartableOnSystemLogin {
                get { return (bool)base["isStartableOnSystemLogin"]; }
                set {
                    base["isStartableOnSystemLogin"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isUriAssociationCheckEnabled", DefaultValue = true)]
            public bool IsUriAssociationCheckEnabled {
                get { return (bool)base["isUriAssociationCheckEnabled"]; }
                set {
                    base["isUriAssociationCheckEnabled"] = value;
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

            [ConfigurationProperty("isRegularAccountBackupEnabled", DefaultValue = false)]
            public bool IsRegularAccountBackupEnabled {
                get { return (bool)base["isRegularAccountBackupEnabled"]; }
                set {
                    base["isRegularAccountBackupEnabled"] = value;
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
            
            [ConfigurationProperty("isRegularWalletBackupEnabled", DefaultValue = false)]
            protected internal bool IsRegularWalletBackupEnabled {
                get { return (bool)base["isRegularWalletBackupEnabled"]; }
                set {
                    base["isRegularWalletBackupEnabled"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("transactionsDefaultFee", DefaultValue = (ulong)0)]
            public ulong TransactionsDefaultFee {
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
                    return !string.IsNullOrEmpty(output) ? output : MoneroAPI.Extensions.Utilities.DefaultPathDirectoryDaemonData;
                }

                set {
                    if (value == MoneroAPI.Extensions.Utilities.DefaultPathDirectoryDaemonData) value = null;
                    base["directoryDaemonData"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("directoryAccountBackups")]
            public string DirectoryAccountBackups {
                get {
                    var output = base["directoryAccountBackups"] as string;
                    return !string.IsNullOrEmpty(output) ? output : MoneroAPI.Extensions.Utilities.DefaultPathDirectoryAccountBackups;
                }

                set {
                    base["directoryAccountBackups"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("fileAccountData")]
            public string FileAccountData {
                get {
                    var output = base["fileAccountData"] as string;
                    return !string.IsNullOrEmpty(output) ? output : MoneroAPI.Extensions.Utilities.DefaultPathFileAccountData;
                }

                set {
                    base["fileAccountData"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareDaemon")]
            public string SoftwareDaemon {
                get {
                    var output = base["softwareDaemon"] as string;
                    return !string.IsNullOrEmpty(output) ? output : MoneroAPI.Extensions.Utilities.DefaultPathSoftwareDaemon;
                }

                set {
                    base["softwareDaemon"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareAccountManager")]
            public string SoftwareAccountManager {
                get {
                    var output = base["softwareAccountManager"] as string;
                    return !string.IsNullOrEmpty(output) ? output : MoneroAPI.Extensions.Utilities.DefaultPathSoftwareAccountManager;
                }

                set {
                    base["softwareAccountManager"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareMiner", DefaultValue = null)]
            protected internal string SoftwareMiner {
                set {
                    base["softwareMiner"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareWallet")]
            protected internal string SoftwareWallet {
                get { return base["softwareWallet"] as string ?? MoneroAPI.Extensions.Utilities.DefaultPathSoftwareAccountManager; }
                set {
                    base["softwareWallet"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("directoryWalletBackups")]
            protected internal string DirectoryWalletBackups {
                get { return base["directoryWalletBackups"] as string ?? MoneroAPI.Extensions.Utilities.DefaultPathDirectoryAccountBackups; }
                set {
                    base["directoryAccountBackups"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("fileWalletData")]
            protected internal string FileWalletData {
                get { return base["fileWalletData"] as string ?? MoneroAPI.Extensions.Utilities.DefaultPathFileAccountData; }
                set {
                    base["fileAccountData"] = value;
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

            [ConfigurationProperty("rpcUrlHostDaemon", DefaultValue = "http://localhost")]
            public string RpcUrlHostDaemon {
                get { return base["rpcUrlHostDaemon"] as string; }
                set {
                    base["rpcUrlHostDaemon"] = value;
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

            [ConfigurationProperty("isProcessDaemonHostedLocally", DefaultValue = true)]
            public bool IsProcessDaemonHostedLocally {
                get { return (bool)base["isProcessDaemonHostedLocally"]; }
                set {
                    base["isProcessDaemonHostedLocally"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("rpcUrlHostAccountManager", DefaultValue = "http://localhost")]
            public string RpcUrlHostAccountManager {
                get { return base["rpcUrlHostAccountManager"] as string; }
                set {
                    base["rpcUrlHostAccountManager"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("rpcUrlPortAccountManager", DefaultValue = (ushort)18082)]
            public ushort RpcUrlPortAccountManager {
                get { return (ushort)base["rpcUrlPortAccountManager"]; }
                set {
                    base["rpcUrlPortAccountManager"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isProcessAccountManagerHostedLocally", DefaultValue = true)]
            public bool IsProcessAccountManagerHostedLocally {
                get { return (bool)base["isProcessAccountManagerHostedLocally"]; }
                set {
                    base["isProcessAccountManagerHostedLocally"] = value;
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

            [ConfigurationProperty("rpcUrlHost", DefaultValue = "localhost")]
            protected internal string RpcUrlHost {
                get { return base["rpcUrlHost"] as string; }
                set {
                    base["rpcUrlHost"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("rpcUrlPortWallet", DefaultValue = (ushort)18082)]
            protected internal ushort RpcUrlPortWallet {
                get { return (ushort)base["rpcUrlPortWallet"]; }
                set {
                    base["rpcUrlPortWallet"] = value;
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

            [ConfigurationProperty("languageCode", DefaultValue = Utilities.DefaultLanguageCode)]
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
                return new ConfigElementContact("", "");
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
