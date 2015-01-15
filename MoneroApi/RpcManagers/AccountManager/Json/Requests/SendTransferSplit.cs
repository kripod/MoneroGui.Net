using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests
{
    public class SendTransferSplit : JsonRpcRequest<SendTransferSplitParameters>
    {
        internal SendTransferSplit(SendTransferSplitParameters parameters) : base("transfer_split", parameters)
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class SendTransferSplitParameters
    {
        [JsonProperty("destinations")]
        private IList<TransferRecipient> Recipients { get; set; }

        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        [JsonProperty("mixin")]
        public ulong MixCount { get; set; }

        [JsonProperty("fee")]
        public ulong Fee {
            get { return 0; }
        }

        [JsonProperty("unlock_time")]
        public ulong UnlockTime { get; set; }

        internal SendTransferSplitParameters(IList<TransferRecipient> recipients)
        {
            Recipients = recipients;
        }
    }
}
