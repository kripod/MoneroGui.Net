using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses
{
    public class Balance
    {
        [JsonProperty("balance")]
        public double Total { get; private set; }

        [JsonProperty("unlocked_balance")]
        public double Spendable { get; private set; }
        public double Unconfirmed {
            get { return Total - Spendable; }
        }

        public Balance(double total, double spendable)
        {
            Total = total;
            Spendable = spendable;
        }
    }
}
