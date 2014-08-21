using System.Net;

namespace Jojatekok.MoneroAPI.Settings
{
    public class RpcSettings
    {
        public string UrlHost { get; set; }
        public ushort UrlPortDaemon { get; set; }
        public ushort UrlPortAccountManager { get; set; }
        public WebProxy Proxy { get; set; }

        public RpcSettings(string urlHost, ushort urlPortDaemon, ushort urlPortAccountManager)
        {
            UrlHost = urlHost;
            UrlPortDaemon = urlPortDaemon;
            UrlPortAccountManager = urlPortAccountManager;
        }
    }
}
