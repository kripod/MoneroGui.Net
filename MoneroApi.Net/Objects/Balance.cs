namespace Jojatekok.MoneroAPI
{
    public class Balance
    {
        public double Total { get; private set; }

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
