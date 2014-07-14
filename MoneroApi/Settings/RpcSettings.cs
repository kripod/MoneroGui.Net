using System.Net;

namespace Jojatekok.MoneroAPI.Settings
{
    public class RpcSettings
    {
        public string UrlHost { get; set; }
        public ushort UrlPortDaemon { get; set; }
        public ushort UrlPortWallet { get; set; }
        public WebProxy Proxy { get; set; }

        public RpcSettings(string urlHost, ushort urlPortDaemon, ushort urlPortWallet)
        {
            UrlHost = urlHost;
            UrlPortDaemon = urlPortDaemon;
            UrlPortWallet = urlPortWallet;
        }
    }
}
