using System;
using Jojatekok.MoneroAPI.ProcessManagers;
using Jojatekok.MoneroAPI.RpcManagers;

namespace Jojatekok.MoneroAPI
{
    public class MoneroClient : IDisposable
    {
        private RpcWebClient RpcWebClient { get; set; }
        private Paths Paths { get; set; }

        public DaemonManager Daemon { get; private set; }
        public WalletManager Wallet { get; private set; }

        private bool IsWalletStartForced { get; set; }

        public MoneroClient(Paths paths)
        {
            RpcWebClient = new RpcWebClient(Helper.RpcUrlBaseIp, Helper.RpcUrlBasePortDaemon, Helper.RpcUrlBasePortWallet);
            Paths = paths;

            Daemon = new DaemonManager(RpcWebClient, Paths);
            Wallet = new WalletManager(RpcWebClient, Daemon, Paths);
        }

        public MoneroClient() : this(new Paths())
        {

        }

        public void StartDaemon()
        {
            Daemon.Start();
        }

        public void StartWallet()
        {
            if (IsWalletStartForced || Wallet.IsWalletFileExistent) {
                IsWalletStartForced = false;
                Wallet.Start();

            } else {
                IsWalletStartForced = true;
                Wallet.RequestPassphrase(true);
            }
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
