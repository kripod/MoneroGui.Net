using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public class JsonRpcRequest
    {
        [JsonProperty("jsonrpc")]
        private static string Version { get { return "2.0"; } }

        [JsonProperty("method")]
        public string Method { get; private set; }

        internal JsonRpcRequest(string method)
        {
            Method = method;
        }
    }

    public abstract class JsonRpcRequest<T> : JsonRpcRequest
    {
        [JsonProperty("params")]
        public T Arguments { get; private set; }

        internal JsonRpcRequest(string method, T arguments) : base(method)
        {
            Arguments = arguments;
        }
    }
}
