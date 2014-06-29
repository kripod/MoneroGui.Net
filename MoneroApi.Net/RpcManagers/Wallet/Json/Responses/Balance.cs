using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses
{
    public class Balance
    {
        private double? _total;
        [JsonProperty("balance")]
        public double? Total {
            get { return _total; }
            private set { _total = value; } // TODO: Divide by 1000000000000 when the RPC is used
        }

        private double? _spendable;
        [JsonProperty("unlocked_balance")]
        public double? Spendable {
            get { return _spendable; }
            private set { _spendable = value; } // TODO: Divide by 1000000000000 when the RPC is used
        }

        public double? Unconfirmed {
            get {
                if (Total == null || Spendable == null) return null;
                return Total.Value - Spendable.Value;
            }
        }

        public Balance(double? total, double? spendable)
        {
            Total = total;
            Spendable = spendable;
        }
    }
}
