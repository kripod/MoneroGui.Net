using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public class JsonRpcResponse<T>
    {
        [JsonProperty("result")]
        public T Result { get; private set; }

        [JsonProperty("error")]
        public JsonError Error { get; private set; }
    }
}
