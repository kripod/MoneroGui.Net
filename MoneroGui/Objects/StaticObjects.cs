using Jojatekok.MoneroAPI;
using Jojatekok.MoneroAPI.Settings;
using Jojatekok.MoneroGUI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
//using System.Net;
using System.Reflection;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Jojatekok.MoneroGUI
{
    static class StaticObjects
    {
        public const string DefaultLanguageCode = "default";

        public const double CoinAtomicValueDivider = 1000000000000;
        public const string StringFormatCoinDisplayValue = "0.000000000000";
        public const string StringFormatCoinBalance = "{0:" + StringFormatCoinDisplayValue + "} {1}";

        // TODO: Fetch this list from the web
        public static readonly List<string> ListExchangeAddresses = new List<string> {
            // Bittrex
            "463tWEBn5XZJSxLU6uLQnQ2iY9xuNcDbjLSjkn3XAXHCbLrTTErJrBWYgHJQyrCwkNgYvyV3z8zctJLPCZy24jvb3NiTcTJ",

            // Bter
            "47CunEQ4v8FPVNnw9mDgNZeaiSo6SVDydB3AZM341ZtdYpBYNmYeqhh4mpU1X6RSmgBTfC8xqaAtUGC2DArotyaKSz1LJyj",

            // HitBTC
            "45VChYXEMP6HhzHzkcZXdJWXazQNRqy8ZKM3zSTiovzbAbhM7P3zQsY3kFjtCNfX9x2Wy9NRRKcxv9M249hUV4bQG8uaD2c",

            // MintPal
            "46aaTzGffy6MmCsY7rQ5CdSAbpPHPj5xkf7yDdfDZSs9YWWEvFhSSkjdr2veqC44q8dt3q1egrLdnZ3oecB1JSMF856eDwb",

            // Poloniex
            "47sghzufGhJJDQEbScMCwVBimTuq6L5JiRixD8VeGbpjCTA12noXmi4ZyBZLc99e66NtnKff34fHsGRoyZk3ES1s1V4QVcB"
        };

        public static readonly Brush BrushForegroundDefault = Brushes.Black;
        public static readonly Brush BrushForegroundWarning = Brushes.OrangeRed;
        public static readonly Brush BrushBackgroundError = Brushes.LightPink;

        public static readonly Assembly ApplicationAssembly = Assembly.GetExecutingAssembly();
        public static readonly AssemblyName ApplicationAssemblyName = ApplicationAssembly.GetName();

        public static readonly Version ApplicationVersionComparable = ApplicationAssemblyName.Version;
        public const string ApplicationVersionExtra = "rc.2";
        public static readonly string ApplicationVersionString = ApplicationVersionComparable.ToString(3) + (ApplicationVersionExtra != null ? "-" + ApplicationVersionExtra : null);

        public static readonly Icon ApplicationIcon = Icon.ExtractAssociatedIcon(ApplicationAssembly.Location);
        public static readonly ImageSource ApplicationIconImage = Icon.ExtractAssociatedIcon(ApplicationAssembly.Location).ToImageSource();

        public static readonly string ApplicationPath = ApplicationAssembly.Location;
        public static readonly string ApplicationStartupShortcutPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            Helper.GetAssemblyAttribute<AssemblyTitleAttribute>().Title + ".lnk"
        );

        public static readonly string ApplicationBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static bool _isUnhandledExceptionLoggingEnabled = true;
        public static bool IsUnhandledExceptionLoggingEnabled {
            get { return _isUnhandledExceptionLoggingEnabled; }
            set { _isUnhandledExceptionLoggingEnabled = value; }
        }

        public static MainWindow MainWindow { get; set; }

        public static MoneroClient MoneroClient { get; private set; }

        public static Logger LoggerDaemon { get; private set; }
        public static Logger LoggerAccountManager { get; private set; }

        public static ObservableCollection<SettingsManager.ConfigElementContact> DataSourceAddressBook { get; private set; }

        public static void Initialize()
        {
            var storedPathSettings = SettingsManager.Paths;
            var pathSettings = new PathSettings {
                DirectoryDaemonData = storedPathSettings.DirectoryDaemonData,
                DirectoryAccountBackups = storedPathSettings.DirectoryAccountBackups,
                FileAccountData = storedPathSettings.FileAccountData,
                SoftwareDaemon = storedPathSettings.SoftwareDaemon,
                SoftwareAccountManager = storedPathSettings.SoftwareAccountManager
            };

            var storedNetworkSettings = SettingsManager.Network;
            var rpcSettings = new RpcSettings(
                storedNetworkSettings.RpcUrlHost,
                storedNetworkSettings.RpcUrlPortDaemon,
                storedNetworkSettings.RpcUrlPortAccountManager
            );

            // TODO: Add support for using proxies
            //if (networkSettings.IsProxyEnabled) {
            //    if (!string.IsNullOrEmpty(networkSettings.ProxyHost) && networkSettings.ProxyPort != null) {
            //        rpcSettings.Proxy = new WebProxy(networkSettings.ProxyHost, (int)networkSettings.ProxyPort);
            //    }
            //}

            MoneroClient = new MoneroClient(pathSettings, rpcSettings);

            LoggerDaemon = new Logger();
            LoggerAccountManager = new Logger();

            DataSourceAddressBook = new ObservableCollection<SettingsManager.ConfigElementContact>(SettingsManager.AddressBook.Elements);
            DataSourceAddressBook.CollectionChanged += DataSourceAddressBook_CollectionChanged;
        }

        private static void DataSourceAddressBook_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Save the collection's changes into the configuration file

            if (DataSourceAddressBook.Count == 0) {
                SettingsManager.AddressBook.Elements.Clear();
                return;
            }

            var oldItems = e.OldItems;
            if (oldItems != null) {
                for (var i = oldItems.Count - 1; i >= 0; i--) {
                    SettingsManager.AddressBook.Elements.Remove(oldItems[i] as SettingsManager.ConfigElementContact);
                }
            }

            var newItems = e.NewItems;
            if (newItems != null) {
                for (var i = newItems.Count - 1; i >= 0; i--) {
                    SettingsManager.AddressBook.Elements.Add(newItems[i] as SettingsManager.ConfigElementContact);
                }
            }
        }
    }
}
