using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests
{
    public class QueryIncomingTransfers : JsonRpcRequest<QueryIncomingTransfersParameters>
    {
        internal QueryIncomingTransfers(string transfersType) : base("incoming_transfers", new QueryIncomingTransfersParameters(transfersType))
        {

        }

        internal QueryIncomingTransfers() : base("incoming_transfers", new QueryIncomingTransfersParameters("all"))
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class QueryIncomingTransfersParameters
    {
        [JsonProperty("transfer_type")]
        private string TransfersType { get; set; }

        internal QueryIncomingTransfersParameters(string transfersType)
        {
            TransfersType = transfersType;
        }
    }
}
