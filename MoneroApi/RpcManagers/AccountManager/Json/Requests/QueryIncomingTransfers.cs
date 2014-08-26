using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests
{
    public class QueryIncomingTransfers : JsonRpcRequest<QueryIncomingTransfersParameters>
    {
        internal QueryIncomingTransfers(QueryIncomingTransfersParameters.TransfersType transfersType) : base("incoming_transfers", new QueryIncomingTransfersParameters(transfersType))
        {

        }

        internal QueryIncomingTransfers() : this(QueryIncomingTransfersParameters.TransfersType.All)
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class QueryIncomingTransfersParameters
    {
        public enum TransfersType
        {
            All,
            Available,
            Unavailable
        }

        [JsonProperty("transfer_type")]
        private string TransfersTypeString { get; set; }

        internal QueryIncomingTransfersParameters(TransfersType transfersType)
        {
            switch (transfersType) {
                case TransfersType.All:
                    TransfersTypeString = "all";
                    break;

                case TransfersType.Available:
                    TransfersTypeString = "available";
                    break;

                case TransfersType.Unavailable:
                    TransfersTypeString = "unavailable";
                    break;
            }
        }
    }
}
