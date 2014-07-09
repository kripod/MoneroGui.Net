using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests
{
    public class GetBlockHeaderByHeight : JsonRpcRequest<GetBlockHeaderByHeightParameters>
    {
        internal GetBlockHeaderByHeight(ulong height) : base("getblockheaderbyheight", new GetBlockHeaderByHeightParameters(height))
        {

        }
    }

    public class GetBlockHeaderByHeightParameters
    {
        [JsonProperty("height")]
        public ulong Height { get; private set; }

        internal GetBlockHeaderByHeightParameters(ulong height)
        {
            Height = height;
        }
    }
}
