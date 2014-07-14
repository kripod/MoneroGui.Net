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
            TextBoxRpcUrlHost.Text = networkSettings.RpcUrlHost;
            IntegerUpDownRpcUrlPortDaemon.Value = networkSettings.RpcUrlPortDaemon;
            IntegerUpDownRpcUrlPortWallet.Value = networkSettings.RpcUrlPortWallet;
            CheckBoxIsProxyEnabled.IsChecked = networkSettings.IsProxyEnabled;
            TextBoxProxyHost.Text = networkSettings.ProxyHost;
            IntegerUpDownProxyPort.Value = networkSettings.ProxyPort;
        }

        public void ApplySettings()
        {
            var networkSettings = SettingsManager.Network;

            if (IntegerUpDownRpcUrlPortDaemon.Value == null) IntegerUpDownRpcUrlPortDaemon.Value = networkSettings.RpcUrlPortDaemon;
            if (IntegerUpDownRpcUrlPortWallet.Value == null) IntegerUpDownRpcUrlPortWallet.Value = networkSettings.RpcUrlPortWallet;
            Debug.Assert(CheckBoxIsProxyEnabled.IsChecked != null, "CheckBoxIsProxyEnabled.IsChecked != null");

            networkSettings.RpcUrlHost = TextBoxRpcUrlHost.Text;
            networkSettings.RpcUrlPortDaemon = (ushort)IntegerUpDownRpcUrlPortDaemon.Value;
            networkSettings.RpcUrlPortWallet = (ushort)IntegerUpDownRpcUrlPortWallet.Value;
            networkSettings.IsProxyEnabled = CheckBoxIsProxyEnabled.IsChecked.Value;
            networkSettings.ProxyHost = TextBoxProxyHost.Text;
            networkSettings.ProxyPort = (ushort?)IntegerUpDownProxyPort.Value;
        }
    }
}
