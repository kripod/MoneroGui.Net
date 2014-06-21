using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Jojatekok.MoneroGUI.Views.MainWindow
{
    sealed class AddressBookViewModel
    {
        public ObservableCollection<SettingsManager.ConfigElementContact> DataSource { get; private set; }

        public AddressBookViewModel()
        {
#if DEBUG
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
#endif

            DataSource = new ObservableCollection<SettingsManager.ConfigElementContact>(SettingsManager.AddressBook.Elements);
            DataSource.CollectionChanged += DataSource_CollectionChanged;
        }

        private void DataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Save the collection's changes into the configuration file

            if (DataSource.Count == 0) {
                SettingsManager.AddressBook.Elements.Clear();
                return;
            }

            var oldItems = e.OldItems;
            if (oldItems != null) {
                for (var i = oldItems.Count - 1; i >= 0; i--) {
                    SettingsManager.AddressBook.Elements.Remove(oldItems[i] as SettingsManager.ConfigElementContact);
                }
            }

            var newItems = e.NewItems;
            if (newItems != null) {
                for (var i = newItems.Count - 1; i >= 0; i--) {
                    SettingsManager.AddressBook.Elements.Add(newItems[i] as SettingsManager.ConfigElementContact);
                }
            }
        }
    }
}
