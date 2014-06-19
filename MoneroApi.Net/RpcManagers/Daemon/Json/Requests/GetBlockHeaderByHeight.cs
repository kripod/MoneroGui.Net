using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests
{
    public class GetBlockHeaderByHeight : JsonRpcRequest<GetBlockHeaderByHeightInternal>
    {
        internal GetBlockHeaderByHeight(ulong height) : base("getblockheaderbyheight", new GetBlockHeaderByHeightInternal(height))
        {

        }
    }

    public class GetBlockHeaderByHeightInternal
    {
        [JsonProperty("height")]
        public ulong Height { get; private set; }

        internal GetBlockHeaderByHeightInternal(ulong height)
        {
            Height = height;
        }
    }
}
