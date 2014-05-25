using System;

namespace Jojatekok.MoneroAPI
{
    public class SyncStatusChangedEventArgs : EventArgs
    {
        public ulong BlocksDownloaded { get; set; }
        public ulong BlocksTotal { get; set; }
        public string StatusText { get; set; }

        public SyncStatusChangedEventArgs(ulong blocksDownloaded, ulong blocksTotal, string statusText)
        {
            BlocksDownloaded = blocksDownloaded;
            BlocksTotal = blocksTotal;
            StatusText = statusText;
        }
    }
}
