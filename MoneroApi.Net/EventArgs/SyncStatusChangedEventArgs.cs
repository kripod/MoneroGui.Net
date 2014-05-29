using System;

namespace Jojatekok.MoneroAPI
{
    public class SyncStatusChangedEventArgs : EventArgs
    {
        public ulong BlocksDownloaded { get; set; }
        public ulong BlocksTotal { get; set; }
        public ulong BlocksRemaining { get; set; }
        public ulong TimeRemainingValue { get; set; }
        public string TimeRemainingText { get; set; }

        public SyncStatusChangedEventArgs(ulong blocksDownloaded, ulong blocksTotal, ulong blocksRemaining, ulong timeRemainingValue, string timeRemainingText)
        {
            BlocksDownloaded = blocksDownloaded;
            BlocksTotal = blocksTotal;
            BlocksRemaining = blocksRemaining;
            TimeRemainingValue = timeRemainingValue;
            TimeRemainingText = timeRemainingText;
        }
    }
}
