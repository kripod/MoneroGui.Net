using Jojatekok.MoneroAPI.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Requests
{
    public class SendTransfer : JsonRpcRequest<SendTransferParameters>
    {
        internal SendTransfer(SendTransferParameters parameters) : base("transfer", parameters)
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class SendTransferParameters
    {
        [JsonProperty("destinations")]
        private IList<TransferRecipient> Recipients { get; set; }

        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        [JsonProperty("mixin")]
        public ulong MixCount { get; set; }

        [JsonProperty("fee")]
        public ulong Fee { get; set; }

        [JsonProperty("unlock_time")]
        public ulong UnlockTime { get; set; }

        internal SendTransferParameters(IList<TransferRecipient> recipients)
        {
            Recipients = recipients;
        }
    }
}
