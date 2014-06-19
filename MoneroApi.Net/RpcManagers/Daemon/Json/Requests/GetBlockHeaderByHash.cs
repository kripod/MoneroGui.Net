using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests
{
    public class GetBlockHeaderByHash : JsonRpcRequest<GetBlockHeaderByHashInternal>
    {
        internal GetBlockHeaderByHash(string hash) : base("getblockheaderbyhash", new GetBlockHeaderByHashInternal(hash))
        {

        }
    }

    public class GetBlockHeaderByHashInternal
    {
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        internal GetBlockHeaderByHashInternal(string hash)
        {
            Hash = hash;
        }
    }
}
