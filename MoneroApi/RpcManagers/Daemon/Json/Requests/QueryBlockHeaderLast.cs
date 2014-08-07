namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests
{
    public class QueryBlockHeaderLast : JsonRpcRequest
    {
        internal QueryBlockHeaderLast() : base("getlastblockheader")
        {

        }
    }
}
