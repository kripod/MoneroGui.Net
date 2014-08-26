using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests
{
    public class QueryKey : JsonRpcRequest<QueryKeyParameters>
    {
        internal QueryKey(QueryKeyParameters.KeyType keyType) : base("query_key", new QueryKeyParameters(keyType))
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class QueryKeyParameters
    {
        public enum KeyType
        {
            ViewKey,
            SpendKey,
            Mnemonic
        }

        [JsonProperty("key_type")]
        private string KeyTypeString { get; set; }

        internal QueryKeyParameters(KeyType keyType)
        {
            switch (keyType) {
                case KeyType.ViewKey:
                    KeyTypeString = "view_key";
                    break;

                case KeyType.SpendKey:
                    KeyTypeString = "spend_key";
                    break;

                case KeyType.Mnemonic:
                    KeyTypeString = "mnemonic";
                    break;
            }
        }
    }
}
