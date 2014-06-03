namespace Jojatekok.MoneroAPI
{
    public class Transaction
    {
        public TransactionType Type { get; private set; }
        public bool IsAmountSpendable { get; private set; }
        public double Amount { get; internal set; }
        public string TransactionId { get; private set; }

        public Transaction(TransactionType type, bool isAmountSpendable, double amount, string transactionId)
        {
            Type = type;
            IsAmountSpendable = isAmountSpendable;
            Amount = amount;
            TransactionId = transactionId;
        }
    }
}
