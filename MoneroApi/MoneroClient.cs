using Jojatekok.MoneroAPI.ProcessManagers;
using Jojatekok.MoneroAPI.RpcManagers;
using System;

namespace Jojatekok.MoneroAPI
{
    public class MoneroClient : IDisposable
    {
        private RpcWebClient RpcWebClient { get; set; }
        private Paths Paths { get; set; }

        public DaemonManager Daemon { get; private set; }
        public WalletManager Wallet { get; private set; }

        public MoneroClient(Paths paths)
        {
            RpcWebClient = new RpcWebClient(Helper.RpcUrlIp, Helper.RpcUrlPortDaemon, Helper.RpcUrlPortWallet);
            Paths = paths;

            Daemon = new DaemonManager(RpcWebClient, Paths);
            Wallet = new WalletManager(RpcWebClient, Paths);
        }

        public MoneroClient() : this(new Paths())
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                if (Wallet != null) {
                    Wallet.Dispose();
                    Wallet = null;
                }

                if (Daemon != null) {
                    Daemon.Dispose();
                    Daemon = null;
                }
            }
        }
    }
}
