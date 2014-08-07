namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Requests
{
    public class QueryBalance : JsonRpcRequest
    {
        public QueryBalance() : base("getbalance")
        {

        }
    }
}
