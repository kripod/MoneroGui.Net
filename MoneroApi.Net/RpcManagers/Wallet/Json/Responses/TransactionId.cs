using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses
{
    public class TransactionId
    {
        private string _value;
        [JsonProperty("tx_hash")]
        public string Value {
            get { return _value; }

            private set {
                // Discard the '<' and '>' characters
                _value = value.Substring(1, value.Length - 2);
            }
        }
    }
}
