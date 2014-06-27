using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses
{
    public class Address
    {
        [JsonProperty("address")]
        public string Value { get; private set; }
    }
}
