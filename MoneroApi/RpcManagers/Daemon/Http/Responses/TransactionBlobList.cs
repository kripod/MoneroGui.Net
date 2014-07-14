using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses
{
    public class TransactionBlobList : RpcResponse
    {
        [JsonProperty("txs_as_hex")]
        private IList<string> FoundTransactionsString {
            set {
                var transactionsCount = value.Count;
                FoundTransactions = new List<Transaction>(transactionsCount);

                for (var i = 0; i < transactionsCount; i++) {
                    var transaction = value[i];
                    // TODO: Deserialize the HEX BLOB into a Transaction
                }
            }
        }
        public IList<Transaction> FoundTransactions { get; private set; }

        [JsonProperty("missed_tx")]
        public IList<string> NotFoundTransactionsString { get; private set; }
        public IList<Transaction> NotFoundTransactions { get; private set; }
    }
}
