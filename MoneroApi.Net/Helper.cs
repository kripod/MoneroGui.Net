using System.Collections.ObjectModel;
using System.Globalization;

namespace Jojatekok.MoneroAPI
{
    static class Helper
    {
        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static void AddOrMergeTransaction(this ObservableCollection<Transaction> collection, Transaction item)
        {
            for (var i = collection.Count - 1; i >= 0; i--) {
                var currentItem = collection[i];
                
                if (currentItem.TransactionId == item.TransactionId && currentItem.IsAmountSpendable == item.IsAmountSpendable) {
                    // TODO: Determine whether checking for the transaction's type is worthless
                    if (currentItem.Type == item.Type) {
                        currentItem.Amount += item.Amount;
                        return;
                    }
                }
            }

            collection.Add(item);
        }
    }
}
