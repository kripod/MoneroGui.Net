using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests
{
    public class GetBlockHeaderByHash : JsonRpcRequest<GetBlockHeaderByHashParameters>
    {
        internal GetBlockHeaderByHash(string hash) : base("getblockheaderbyhash", new GetBlockHeaderByHashParameters(hash))
        {

        }
    }

    public class GetBlockHeaderByHashParameters
    {
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        internal GetBlockHeaderByHashParameters(string hash)
        {
            Hash = hash;
        }
    }
}
