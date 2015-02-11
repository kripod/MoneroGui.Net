using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class NetworkView : ISettingsView
    {
        public NetworkView()
        {
            InitializeComponent();

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
#endif

            // Load settings
            var networkSettings = SettingsManager.Network;

            TextBoxRpcUrlHostDaemon.Text = networkSettings.RpcUrlHostDaemon;
            IntegerUpDownRpcUrlPortDaemon.Value = networkSettings.RpcUrlPortDaemon;
            CheckBoxIsProcessDaemonHostedLocally.IsChecked = networkSettings.IsProcessDaemonHostedLocally;

            TextBoxRpcUrlHostAccountManager.Text = networkSettings.RpcUrlHostAccountManager;
            IntegerUpDownRpcUrlPortAccountManager.Value = networkSettings.RpcUrlPortAccountManager;
            CheckBoxIsProcessAccountManagerHostedLocally.IsChecked = networkSettings.IsProcessAccountManagerHostedLocally;

            CheckBoxIsProxyEnabled.IsChecked = networkSettings.IsProxyEnabled;
            TextBoxProxyHost.Text = networkSettings.ProxyHost;
            IntegerUpDownProxyPort.Value = networkSettings.ProxyPort;
        }

        private bool IsHostValid(string input)
        {
            Uri uri;
            if (Uri.TryCreate(input, UriKind.Absolute, out uri)) {
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) {
                    return true;
                }
            }

            return false;
        }

        public void ApplySettings()
        {
            var networkSettings = SettingsManager.Network;

            if (IntegerUpDownRpcUrlPortDaemon.Value == null) IntegerUpDownRpcUrlPortDaemon.Value = networkSettings.RpcUrlPortDaemon;
            if (IntegerUpDownRpcUrlPortAccountManager.Value == null) IntegerUpDownRpcUrlPortAccountManager.Value = networkSettings.RpcUrlPortAccountManager;
            Debug.Assert(CheckBoxIsProcessDaemonHostedLocally.IsChecked != null, "CheckBoxIsProcessDaemonHostedLocally.IsChecked != null");
            Debug.Assert(CheckBoxIsProcessAccountManagerHostedLocally.IsChecked != null, "CheckBoxIsProcessAccountManagerHostedLocally.IsChecked != null");
            Debug.Assert(CheckBoxIsProxyEnabled.IsChecked != null, "CheckBoxIsProxyEnabled.IsChecked != null");

            var hostDaemon = TextBoxRpcUrlHostDaemon.Text;
            if (IsHostValid(hostDaemon)) {
                networkSettings.RpcUrlHostDaemon = hostDaemon;
            }
            networkSettings.RpcUrlPortDaemon = (ushort)IntegerUpDownRpcUrlPortDaemon.Value;
            networkSettings.IsProcessDaemonHostedLocally = CheckBoxIsProcessDaemonHostedLocally.IsChecked.Value;

            var hostAccountManager = TextBoxRpcUrlHostAccountManager.Text;
            if (IsHostValid(hostAccountManager)) {
                networkSettings.RpcUrlHostAccountManager = hostAccountManager;
            }
            networkSettings.RpcUrlPortAccountManager = (ushort)IntegerUpDownRpcUrlPortAccountManager.Value;
            networkSettings.IsProcessAccountManagerHostedLocally = CheckBoxIsProcessAccountManagerHostedLocally.IsChecked.Value;

            networkSettings.IsProxyEnabled = CheckBoxIsProxyEnabled.IsChecked.Value;
            networkSettings.ProxyHost = TextBoxProxyHost.Text;
            networkSettings.ProxyPort = (ushort?)IntegerUpDownProxyPort.Value;
        }
    }
}
