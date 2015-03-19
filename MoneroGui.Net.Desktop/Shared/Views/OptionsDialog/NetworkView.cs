using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI.Views.OptionsDialog
{
    public class NetworkView : TableLayout, IOptionsTabPageView, INotifyPropertyChanged
    {
        private ushort _proxyPort;

        private RpcServerSettingsManager RpcServerSettingsManagerDaemon { get; set; }
        private RpcServerSettingsManager RpcServerSettingsManagerAccountManager { get; set; }

        private CheckBox CheckBoxIsProxyEnabled { get; set; }
        private TextBox TextBoxProxyHost { get; set; }
        private NumericUpDown NumericUpDownProxyPort { get; set; }

        private ushort ProxyPort {
            get { return _proxyPort; }
            set {
                _proxyPort = value;
                OnPropertyChanged();
            }
        }

        public NetworkView()
        {
            LoadSettings();

            Spacing = Utilities.Spacing3;

            Rows.Add(new GroupBox {
                Text = MoneroGUI.Properties.Resources.OptionsNetworkRpcServerSettings,
                Content = new TableLayout(
                    RpcServerSettingsManagerDaemon,
                    RpcServerSettingsManagerAccountManager
                ) { Padding = new Padding(Utilities.Padding2), Spacing = Utilities.Spacing3 }
            });

            Rows.Add(new GroupBox {
                Text = MoneroGUI.Properties.Resources.OptionsNetworkProxySettings,
                Content = new TableLayout(
                    CheckBoxIsProxyEnabled,

                    new TableLayout(
                        new TableRow(
                            new Label { Text = MoneroGUI.Properties.Resources.TextHost },
                            new TableCell(TextBoxProxyHost, true),

                            new Separator(SeparatorOrientation.Vertical),

                            new Label { Text = MoneroGUI.Properties.Resources.TextPort },
                            NumericUpDownProxyPort
                        )
                    ) { Spacing = Utilities.Spacing3 }
                ) { Padding = new Padding(Utilities.Padding2), Spacing = Utilities.Spacing3 }
            });

            Rows.Add(new TableRow());
        }

        void LoadSettings()
        {
            var networkSettings = SettingsManager.Network;

            RpcServerSettingsManagerDaemon = new RpcServerSettingsManager(
                MoneroGUI.Properties.Resources.TextDaemon,
                networkSettings.RpcUrlHostDaemon,
                networkSettings.RpcUrlPortDaemon,
                networkSettings.IsProcessDaemonHostedLocally
            );

            RpcServerSettingsManagerAccountManager = new RpcServerSettingsManager(
                MoneroGUI.Properties.Resources.TextAccountManager,
                networkSettings.RpcUrlHostAccountManager,
                networkSettings.RpcUrlPortAccountManager,
                networkSettings.IsProcessAccountManagerHostedLocally
            );

            CheckBoxIsProxyEnabled = new CheckBox {
                Text = MoneroGUI.Properties.Resources.OptionsNetworkIsProxyEnabled,
                Checked = networkSettings.IsProxyEnabled
            };
            CheckBoxIsProxyEnabled.CheckedChanged += OnCheckBoxIsProxyEnabledCheckedChanged;

            TextBoxProxyHost = new TextBox { Text = networkSettings.ProxyHost };

            NumericUpDownProxyPort = Utilities.CreateNumericUpDown(this, o => o.ProxyPort, 0, 1, ushort.MaxValue);
            ProxyPort = networkSettings.ProxyPort;

            OnCheckBoxIsProxyEnabledCheckedChanged(null, null);
        }

        void OnCheckBoxIsProxyEnabledCheckedChanged(object sender, EventArgs e)
        {
            Debug.Assert(CheckBoxIsProxyEnabled.Checked != null, "CheckBoxIsProxyEnabled.Checked != null");
            var isProxyEnabled = CheckBoxIsProxyEnabled.Checked.Value;

            TextBoxProxyHost.Enabled = isProxyEnabled;
            NumericUpDownProxyPort.Enabled = isProxyEnabled;
        }

        public void ApplySettings()
        {
            var networkSettings = SettingsManager.Network;

            Debug.Assert(CheckBoxIsProxyEnabled.Checked != null, "CheckBoxIsProxyEnabled.Checked != null");

            var hostDaemon = RpcServerSettingsManagerDaemon.RpcUrlHost;
            if (Utilities.IsHostUrlValid(hostDaemon)) {
                networkSettings.RpcUrlHostDaemon = hostDaemon;
            }
            networkSettings.RpcUrlPortDaemon = RpcServerSettingsManagerDaemon.RpcUrlPort;
            networkSettings.IsProcessDaemonHostedLocally = RpcServerSettingsManagerDaemon.IsProcessHostedLocally;

            var hostAccountManager = RpcServerSettingsManagerAccountManager.RpcUrlHost;
            if (Utilities.IsHostUrlValid(hostAccountManager)) {
                networkSettings.RpcUrlHostAccountManager = hostAccountManager;
            }
            networkSettings.RpcUrlPortAccountManager = RpcServerSettingsManagerAccountManager.RpcUrlPort;
            networkSettings.IsProcessAccountManagerHostedLocally = RpcServerSettingsManagerAccountManager.IsProcessHostedLocally;

            networkSettings.IsProxyEnabled = CheckBoxIsProxyEnabled.Checked.Value;
            networkSettings.ProxyHost = TextBoxProxyHost.Text;
            networkSettings.ProxyPort = ProxyPort;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
