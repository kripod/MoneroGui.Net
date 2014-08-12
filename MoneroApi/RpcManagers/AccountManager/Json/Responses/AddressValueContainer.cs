using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses
{
    class AddressValueContainer : IValueContainer<string>
    {
        [JsonProperty("address")]
        public string Value { get; private set; }
    }
}
