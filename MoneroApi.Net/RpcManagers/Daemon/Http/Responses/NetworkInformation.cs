using System;
using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses
{
    public class NetworkInformation : RpcResponse
    {
        [JsonProperty("alt_blocks_count")]
        public int AlternativeBlockCount { get; private set; }

        [JsonProperty("difficulty")]
        public ulong BlockDifficulty { get; private set; }
        [JsonProperty("height")]
        public ulong BlockHeightDownloaded { get; private set; }
        [JsonProperty("target_height")]
        public ulong BlockHeightTotal { get; private set; }
        public ulong BlockHeightRemaining {
            get {
                if (BlockHeightDownloaded > BlockHeightTotal) return 0;
                return BlockHeightTotal - BlockHeightDownloaded;
            }
        }

        public DateTime BlockTimeCurrent { get; internal set; }
        public TimeSpan BlockTimeRemaining {
            get { return DateTime.UtcNow.Subtract(BlockTimeCurrent); }
        }

        [JsonProperty("grey_peerlist_size")]
        public ulong PeerListSizeGrey { get; private set; }
        [JsonProperty("white_peerlist_size")]
        public ulong PeerListSizeWhite { get; private set; }

        [JsonProperty("incoming_connections_count")]
        public ushort ConnectionCountIncoming { get; private set; }
        [JsonProperty("outgoing_connections_count")]
        public ushort ConnectionCountOutgoing { get; private set; }

        [JsonProperty("tx_count")]
        public ulong TransactionCountTotal { get; private set; }
        [JsonProperty("tx_pool_size")]
        public uint TransactionPoolSize { get; private set; }
    }
}
