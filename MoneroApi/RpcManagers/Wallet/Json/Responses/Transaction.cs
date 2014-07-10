using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses
{
    class TransactionList : RpcResponse
    {
        [JsonProperty("transfers")]
        public IList<Transaction> Value { get; private set; }
    }

    public class Transaction
    {
        [JsonProperty("amount")]
        public ulong Amount { get; internal set; }

        private string _transactionId;
        [JsonProperty("tx_hash")]
        public string TransactionId {
            get { return _transactionId; }

            internal set {
                // Discard the '<' and '>' characters
                _transactionId = value.Substring(1, value.Length - 2);
            }
        }

        private bool _isAmountSpendable;
        [JsonProperty("spent")]
        public bool IsAmountSpendable {
            get { return _isAmountSpendable; }
            internal set { _isAmountSpendable = !value; }
        }

        public TransactionType Type { get; internal set; }

        public uint Number { get; internal set; }

        [JsonProperty("global_index")]
        public ulong GlobalIndex { get; internal set; }
    }
}
