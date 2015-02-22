using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroAPI.Extensions;
using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;

namespace Jojatekok.MoneroGUI
{
    static class Utilities
    {
        public const string DefaultLanguageCode = "default";

        public const byte FontSize2 = 12;
        public const byte FontSize3 = 20;

        public const byte Padding1 = 3;
        public const byte Padding2 = 5;
        public const byte Padding3 = 8;
        public const byte Padding4 = 10;
        public const byte Padding5 = 14;
        public const byte Padding6 = 20;
        public const byte Padding7 = 30;

        public static readonly string PathDirectoryThirdPartyLicenses = new DirectoryInfo("Licenses").FullName;
        public static readonly string PathFileLicense = new FileInfo("LICENSE").FullName;

        public static readonly Color ColorSeparator = Color.FromRgb(10526880);
        public static readonly Color ColorStatusBar = Color.FromRgb(15855085);

        public static readonly Size Spacing2 = new Size(Padding2, Padding2);
        public static readonly Size Spacing3 = new Size(Padding3, Padding3);
        public static readonly Size Spacing5 = new Size(Padding5, Padding5);

        public static readonly BindingCollection BindingsToAccountAddress = new BindingCollection();
        public static readonly BindingCollection BindingsToAccountBalance = new BindingCollection();
        public static readonly BindingCollection BindingsToAccountTransactions = new BindingCollection();

        public static readonly ObservableCollection<Transaction> AccountTransactions = new ObservableCollection<Transaction>();

        public static readonly Assembly ApplicationAssembly = Assembly.GetExecutingAssembly();
        public static readonly AssemblyName ApplicationAssemblyName = ApplicationAssembly.GetName();
        public static readonly string ApplicationAssemblyNameName = ApplicationAssembly.GetName().Name;

        public static readonly Version ApplicationVersionComparable = ApplicationAssemblyName.Version;
        public const string ApplicationVersionExtra = null;
        public static readonly string ApplicationVersionString = ApplicationVersionComparable.ToString(3) + (ApplicationVersionExtra != null ? "-" + ApplicationVersionExtra : null);

        private static readonly ImageConverter ImageConverter = new ImageConverter();

        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static SynchronizationContext SyncContextMain { get; set; }

        public static MoneroProcessManager MoneroProcessManager { get; private set; }
        public static MoneroRpcManager MoneroRpcManager { get; private set; }

        public static void Initialize()
        {
            SyncContextMain = SynchronizationContext.Current;
		    using (var button = new Button()) {
		        var handler = button.Handler;

                var fieldInfo = handler.GetType().GetField("MinimumSize");
		        if (fieldInfo != null) {
		            var size = (Size)(fieldInfo.GetValue(null));
		            size.Width = 0;
                    fieldInfo.SetValue(null, size);

		        } else {
                    fieldInfo = handler.GetType().GetField("MinimumWidth");
		            if (fieldInfo != null) {
		                fieldInfo.SetValue(null, 0);
		            }
		        }
		    }

            SettingsManager.Initialize();

            var storedPathSettings = SettingsManager.Paths;
            var daemonProcessSettings = new DaemonProcessSettings {
                SoftwareDaemon = storedPathSettings.SoftwareDaemon,
                DirectoryDaemonData = storedPathSettings.DirectoryDaemonData,
            };
            var accountManagerProcessSettings = new AccountManagerProcessSettings {
                SoftwareAccountManager = storedPathSettings.SoftwareAccountManager,
                DirectoryAccountBackups = storedPathSettings.DirectoryAccountBackups,
                FileAccountData = storedPathSettings.FileAccountData,
            };

            var storedNetworkSettings = SettingsManager.Network;
            var rpcSettings = new RpcSettings(
                storedNetworkSettings.RpcUrlHostDaemon,
                storedNetworkSettings.RpcUrlPortDaemon,
                storedNetworkSettings.RpcUrlHostAccountManager,
                storedNetworkSettings.RpcUrlPortAccountManager
            );
            if (storedNetworkSettings.IsProxyEnabled) {
                if (!string.IsNullOrEmpty(storedNetworkSettings.ProxyHost) && storedNetworkSettings.ProxyPort != null) {
                    rpcSettings.Proxy = new WebProxy(storedNetworkSettings.ProxyHost, (int)storedNetworkSettings.ProxyPort);
                }
            }

            MoneroProcessManager = new MoneroProcessManager(rpcSettings, accountManagerProcessSettings, daemonProcessSettings);
            MoneroRpcManager = new MoneroRpcManager(rpcSettings);
        }

        public static Image LoadImage(string resourceName)
        {
            return ImageConverter.ConvertFrom(
                ImageConverter.ResourcePrefix +
                "Jojatekok.MoneroGUI." + resourceName + ".png," +
                ApplicationAssemblyNameName
            ) as Image;
        }

        public static Label CreateLabel(Func<string> textBinding, HorizontalAlign horizontalAlignment = HorizontalAlign.Left, VerticalAlign verticalAlignment = VerticalAlign.Middle, Font font = null)
        {
            var label = new Label {
                HorizontalAlign = horizontalAlignment,
                VerticalAlign = verticalAlignment
            };

            label.SetTextBindingPath(textBinding);
            if (font != null) label.Font = font;

            return label;
        }

        public static Label CreateLabel<T>(T dataContext, Expression<Func<T, string>> textBinding, HorizontalAlign horizontalAlignment = HorizontalAlign.Left, VerticalAlign verticalAlignment = VerticalAlign.Middle, Font font = null)
        {
            var label = new Label {
                DataContext = dataContext,
                HorizontalAlign = horizontalAlignment,
                VerticalAlign = verticalAlignment
            };

            label.TextBinding.BindDataContext(textBinding);
            if (font != null) label.Font = font;

            return label;
        }

        public static TextBox CreateTextBox(Func<string> placeholderTextBinding, string text = null, Font font = null)
        {
            var textBox = new TextBox {
                Text = text
            };

            textBox.SetPlaceholderTextBindingPath(placeholderTextBinding);
            if (font != null) textBox.Font = font;

            return textBox;
        }

        public static Button CreateButton(Func<string> textBinding, Image image = null)
        {
            var button = new Button {
                Image = image
            };

            button.SetTextBindingPath(textBinding);

            return button;
        }

        public static NumericUpDown CreateNumericUpDown<T>(int decimalPlaces, double increment, double minValue, double maxValue, T dataContext, Expression<Func<T, double>> valueBinding)
        {
            var numericUpDown = new NumericUpDown {
                DecimalPlaces = decimalPlaces,
                Increment = increment,
                MinValue = minValue,
                MaxValue = maxValue,
                DataContext = dataContext
            };

            numericUpDown.ValueBinding.BindDataContext(valueBinding);

            if (decimalPlaces == 0) {
                numericUpDown.ValueChanged += (sender, e) => {
                    numericUpDown.Value = valueBinding.Compile().Invoke(dataContext);
                };
            }

            return numericUpDown;
        }

        public static GridView CreateGridView<T>(FilterCollection<T> dataStore, params GridColumn[] columns) where T : class
        {
            var gridView = new GridView<T> {
                DataStore = dataStore,
                ShowCellBorders = true
            };

            //dataStore.Change = () => gridView.SelectionPreserver;

            for (var i = 0; i < columns.Length; i++) {
                gridView.Columns.Add(columns[i]);
            }

            return gridView;
        }

        public static T GetAssemblyAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(ApplicationAssembly, typeof(T), false);
        }
    }
}
