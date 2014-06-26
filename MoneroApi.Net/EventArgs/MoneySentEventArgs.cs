using System;

namespace Jojatekok.MoneroAPI
{
    public class MoneySentEventArgs : EventArgs
    {
        public string TransactionId { get; private set; }

        internal MoneySentEventArgs(string transactionId)
        {
            TransactionId = transactionId;
        }
    }
}
