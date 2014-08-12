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
            IntegerUpDownRpcUrlPortAccountManager.Value = networkSettings.RpcUrlPortAccountManager;
            CheckBoxIsProxyEnabled.IsChecked = networkSettings.IsProxyEnabled;
            TextBoxProxyHost.Text = networkSettings.ProxyHost;
            IntegerUpDownProxyPort.Value = networkSettings.ProxyPort;
        }

        public void ApplySettings()
        {
            var networkSettings = SettingsManager.Network;

            if (IntegerUpDownRpcUrlPortDaemon.Value == null) IntegerUpDownRpcUrlPortDaemon.Value = networkSettings.RpcUrlPortDaemon;
            if (IntegerUpDownRpcUrlPortAccountManager.Value == null) IntegerUpDownRpcUrlPortAccountManager.Value = networkSettings.RpcUrlPortAccountManager;
            Debug.Assert(CheckBoxIsProxyEnabled.IsChecked != null, "CheckBoxIsProxyEnabled.IsChecked != null");

            networkSettings.RpcUrlHost = TextBoxRpcUrlHost.Text;
            networkSettings.RpcUrlPortDaemon = (ushort)IntegerUpDownRpcUrlPortDaemon.Value;
            networkSettings.RpcUrlPortAccountManager = (ushort)IntegerUpDownRpcUrlPortAccountManager.Value;
            networkSettings.IsProxyEnabled = CheckBoxIsProxyEnabled.IsChecked.Value;
            networkSettings.ProxyHost = TextBoxProxyHost.Text;
            networkSettings.ProxyPort = (ushort?)IntegerUpDownProxyPort.Value;
        }
    }
}
