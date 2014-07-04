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

        private ulong _feeAtomicValue = 5000000000; // TODO: Allow custom values for transaction fees
        [JsonProperty("fee")]
        private ulong FeeAtomicValue {
            get { return _feeAtomicValue; }
            set { _feeAtomicValue = value; }
        }

        public double Fee {
            get { return FeeAtomicValue / Helper.CoinAtomicValueDivider; }
            set { FeeAtomicValue = (ulong)(value * Helper.CoinAtomicValueDivider); }
        }

        [JsonProperty("mixin")]
        public ulong MixCount { get; set; }

        [JsonProperty("unlock_time")]
        public ulong UnlockTime { get; set; }

        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        internal SendTransferParameters(IList<TransferRecipient> recipients)
        {
            Recipients = recipients;
        }
    }
}
