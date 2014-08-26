using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses
{
    class KeyValueContainer : IValueContainer<string>
    {
        [JsonProperty("key")]
        public string Value { get; private set; }
    }
}
