using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

namespace Jojatekok.MoneroAPI
{
    public class ConcurrentReadOnlyObservableCollection<T> : ReadOnlyObservableCollection<T>
    {
        protected override event NotifyCollectionChangedEventHandler CollectionChanged;

        public ConcurrentReadOnlyObservableCollection(ObservableCollection<T> collection) : base(collection)
        {
            
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged == null) return;

            var delegates = CollectionChanged.GetInvocationList();
            for (var i = delegates.Length - 1; i >= 0; i--) {
                var handler = delegates[i] as NotifyCollectionChangedEventHandler;
                Debug.Assert(handler != null, "handler != null");

                var dispatcherObject = handler.Target as DispatcherObject;

                if (dispatcherObject != null && !dispatcherObject.CheckAccess()) {
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                } else {
                    handler(this, e);
                }
            }
        }
    }
}
