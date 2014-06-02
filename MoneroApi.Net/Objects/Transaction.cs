namespace Jojatekok.MoneroAPI
{
    public class Transaction
    {
        public TransactionType Type { get; private set; }
        public double Amount { get; private set; }
        public string TransactionId { get; private set; }

        public Transaction(TransactionType type, double amount, string transactionId)
        {
            Type = type;
            Amount = amount;
            TransactionId = transactionId;
        }
    }
}
