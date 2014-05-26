using System;
using Jojatekok.MoneroAPI.ProcessManagers;

namespace Jojatekok.MoneroAPI
{
    public class MoneroClient : IDisposable
    {
        public DaemonManager Daemon { get; private set; }
        public WalletManager Wallet { get; private set; }

        public MoneroClient(Paths paths)
        {
            Daemon = new DaemonManager(paths);
            Wallet = new WalletManager(paths);
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
                if (Daemon != null) {
                    Daemon.Dispose();
                    Daemon = null;
                }
            }
        }
    }
}
