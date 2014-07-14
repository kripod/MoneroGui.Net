using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Requests
{
    public class GetTransactions : HttpRpcRequest
    {
        [JsonProperty("txs_hashes")]
        private IList<string> TransactionIds { get; set; }

        internal GetTransactions(IList<string> transactionIds)
        {
            TransactionIds = transactionIds;
        }
    }
}
