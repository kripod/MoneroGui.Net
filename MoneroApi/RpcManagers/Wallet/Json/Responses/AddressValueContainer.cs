using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses
{
    class AddressValueContainer : IValueContainer<string>
    {
        [JsonProperty("address")]
        public string Value { get; private set; }
    }
}
