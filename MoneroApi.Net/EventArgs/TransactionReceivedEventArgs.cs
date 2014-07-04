using Jojatekok.MoneroAPI.RpcManagers.Wallet.Json.Responses;
using System;

namespace Jojatekok.MoneroAPI
{
    public class TransactionReceivedEventArgs : EventArgs
    {
        public Transaction Transaction { get; private set; }

        internal TransactionReceivedEventArgs(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}
