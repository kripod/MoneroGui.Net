using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests
{
    public class QueryBlockHeaderByHeight : JsonRpcRequest<QueryBlockHeaderByHeightParameters>
    {
        internal QueryBlockHeaderByHeight(ulong height) : base("getblockheaderbyheight", new QueryBlockHeaderByHeightParameters(height))
        {

        }
    }

    public class QueryBlockHeaderByHeightParameters
    {
        [JsonProperty("height")]
        public ulong Height { get; private set; }

        internal QueryBlockHeaderByHeightParameters(ulong height)
        {
            Height = height;
        }
    }
}
