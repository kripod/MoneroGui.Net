using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests
{
    public class QueryBlockHeaderByHash : JsonRpcRequest<QueryBlockHeaderByHashParameters>
    {
        internal QueryBlockHeaderByHash(string hash) : base("getblockheaderbyhash", new QueryBlockHeaderByHashParameters(hash))
        {

        }
    }

    public class QueryBlockHeaderByHashParameters
    {
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        internal QueryBlockHeaderByHashParameters(string hash)
        {
            Hash = hash;
        }
    }
}
