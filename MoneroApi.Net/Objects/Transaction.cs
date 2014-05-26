namespace Jojatekok.MoneroAPI
{
    public class Transaction
    {
        public double Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public string TransactionId { get; private set; }

        public Transaction(double amount, TransactionType type, string transactionId)
        {
            Amount = amount;
            Type = type;
            TransactionId = transactionId;
        }
    }
}
