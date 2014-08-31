using Jojatekok.MoneroAPI.ProcessManagers;
using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.Settings;
using System;

namespace Jojatekok.MoneroAPI
{
    public class MoneroClient : IDisposable
    {
        private RpcWebClient RpcWebClient { get; set; }
        private PathSettings Paths { get; set; }

        public DaemonManager Daemon { get; private set; }
        public AccountManager AccountManager { get; private set; }

        public MoneroClient(PathSettings paths, RpcSettings rpcSettings)
        {
            RpcWebClient = new RpcWebClient(rpcSettings);
            Paths = paths;

            Daemon = new DaemonManager(RpcWebClient, Paths);
            AccountManager = new AccountManager(RpcWebClient, Paths);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                if (AccountManager != null) {
                    AccountManager.Dispose();
                    AccountManager = null;
                }

                if (Daemon != null) {
                    Daemon.Dispose();
                    Daemon = null;
                }
            }
        }
    }
}
