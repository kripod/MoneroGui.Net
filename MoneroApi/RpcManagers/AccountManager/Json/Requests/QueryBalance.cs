namespace Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests
{
    public class QueryBalance : JsonRpcRequest
    {
        public QueryBalance() : base("getbalance")
        {

        }
    }
}
