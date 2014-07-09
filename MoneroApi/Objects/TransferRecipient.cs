using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TransferRecipient
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("amount")]
        private ulong AmountAtomicValue { get; set; }
        public double Amount {
            get { return AmountAtomicValue / Helper.CoinAtomicValueDivider; }
            set { AmountAtomicValue = (ulong)(value * Helper.CoinAtomicValueDivider); }
        }

        public TransferRecipient(string address, double amount)
        {
            Address = address;
            Amount = amount;
        }
    }
}
