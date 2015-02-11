using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Jojatekok.MoneroGUI
{
    public sealed class ObservableCollectionEx<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ItemChanged;

        public ObservableCollectionEx()
        {
            CollectionChanged += ObservableCollectionEx_CollectionChanged;
        }

        private void ObservableCollectionEx_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null) {
                for (var i = e.NewItems.Count - 1; i >= 0; i--) {
                    var item = e.NewItems[i] as INotifyPropertyChanged;
                    if (item != null) {
                        item.PropertyChanged += Item_PropertyChanged;
                    }
                }
            }

            if (e.OldItems != null) {
                for (var i = e.OldItems.Count - 1; i >= 0; i--) {
                    var item = e.OldItems[i] as INotifyPropertyChanged;
                    if (item != null) {
                        item.PropertyChanged -= Item_PropertyChanged;
                    }
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ItemChanged != null) ItemChanged(this, e);
        }
    }
}
