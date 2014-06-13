using System;
using Jojatekok.MoneroAPI.ProcessManagers;

namespace Jojatekok.MoneroAPI
{
    public class MoneroClient : IDisposable
    {
        public DaemonManager Daemon { get; private set; }
        public WalletManager Wallet { get; private set; }

        public MoneroClient(Paths paths, string password)
        {
            Daemon = new DaemonManager(paths);
            Wallet = new WalletManager(Daemon, paths, password);
        }

        public MoneroClient(Paths paths) : this(paths, null)
        {

        }

        public MoneroClient(string password) : this(new Paths(), password)
        {

        }

        public MoneroClient() : this(new Paths(), null)
        {

        }

        public void Start()
        {
            Daemon.Start();
            Wallet.Start();
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
