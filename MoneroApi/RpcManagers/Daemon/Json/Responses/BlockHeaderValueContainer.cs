using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Responses
{
    class BlockHeaderValueContainer : IValueContainer<BlockHeader>
    {
        [JsonProperty("block_header")]
        public BlockHeader Value { get; private set; }
    }
}
