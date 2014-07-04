using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Requests
{
    public class GetIncomingTransfers : JsonRpcRequest<GetIncomingTransfersParameters>
    {
        internal GetIncomingTransfers(string transfersType) : base("incoming_transfers", new GetIncomingTransfersParameters(transfersType))
        {

        }

        internal GetIncomingTransfers() : base("incoming_transfers", new GetIncomingTransfersParameters("all"))
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class GetIncomingTransfersParameters
    {
        [JsonProperty("transfer_type")]
        public string TransfersType { get; set; }

        internal GetIncomingTransfersParameters(string transfersType)
        {
            TransfersType = transfersType;
        }
    }
}
