using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = System.Drawing.Image;

namespace Jojatekok.MoneroGUI
{
    static class Helper
    {
        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        public static readonly CultureInfo DefaultUiCulture = CultureInfo.InstalledUICulture;

        public static readonly string NewLineString = Environment.NewLine;

        public static string UppercaseFirst(this string input)
        {
            return char.ToUpper(input[0], InvariantCulture) + input.Substring(1);
        }

        public static string ReWrap(this string input)
        {
            return Regex.Replace(input.TrimEnd(), " (\r\n|\n)", " ");
        }

        public static string ToStringReadable(this TimeSpan timeSpan)
        {
            var days = timeSpan.Days;
            if (days > 0) {
                if (days == 1) return "1 " + Properties.Resources.StatusBarSyncTextDaySingular;
                return days + " " + Properties.Resources.StatusBarSyncTextDayPlural;
            }

            var hours = timeSpan.Hours;
            if (hours > 0) {
                if (hours == 1) return "1 " + Properties.Resources.StatusBarSyncTextHourSingular;
                return hours + " " + Properties.Resources.StatusBarSyncTextHourPlural;
            }

            var minutes = timeSpan.Minutes;
            if (minutes == 1) return "1 " + Properties.Resources.StatusBarSyncTextMinuteSingular;
            return minutes + " " + Properties.Resources.StatusBarSyncTextMinutePlural;
        }

        public static void SetFocusedElement(this Control control, FrameworkElement element)
        {
            control.Dispatcher.BeginInvoke(new Action(() => element.Focus()), DispatcherPriority.ContextIdle);
        }

        public static void SetFocusToParent(this Control control)
        {
            control.SetFocusedElement((FrameworkElement)control.Parent);
        }

        public static void SetDefaultFocusedElement(this Control control, FrameworkElement element)
        {
            control.IsVisibleChanged += ((sender, e) => {
                if ((bool)e.NewValue) {
                    control.SetFocusedElement(element);
                }
            });
        }

        public static void SetDefaultFocusToParent(this Control control)
        {
            control.IsVisibleChanged += ((sender, e) => {
                if ((bool)e.NewValue) {
                    control.SetFocusToParent();
                }
            });
        }

        public static void ActivateWindowOrLastChild(this Window window)
        {
            var ownedWindows = window.OwnedWindows;
            if (ownedWindows.Count == 0) {
                if (window.Visibility == Visibility.Visible) window.Activate();

            } else {
                var lastOwnedWindow = ownedWindows[ownedWindows.Count - 1];

                Debug.Assert(lastOwnedWindow != null, "lastOwnedWindow != null");
                lastOwnedWindow.Activate();
            }
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static BitmapImage ToBitmapImage(this Image image, bool isTransparent)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            var stream = new MemoryStream();
            image.Save(stream, isTransparent ? ImageFormat.Png : ImageFormat.Bmp);

            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            using (var stream = new MemoryStream()) {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                return new Bitmap(stream);
            }
        }

        public static int IndexOfLabel(this IList<SettingsManager.ConfigElementContact> collection, string label)
        {
            for (var i = collection.Count - 1; i >= 0; i--) {
                if (collection[i].Label == label) {
                    return i;
                }
            }

            return -1;
        }

        public static int IndexOfAddress(this IList<SettingsManager.ConfigElementContact> collection, string address)
        {
            for (var i = collection.Count - 1; i >= 0; i--) {
                if (collection[i].Address == address) {
                    return i;
                }
            }

            return -1;
        }

        private static string GetPathLastPart(string path)
        {
            var lastSlashIndex = path.LastIndexOf('\\');
            if (lastSlashIndex >= 0) return path.Substring(lastSlashIndex + 1);
            
            return string.Empty;
        }

        public static string GetDirectoryOfFile(string path)
        {
            var lastSlashIndex = path.LastIndexOf('\\');
            if (lastSlashIndex < 0) return StaticObjects.ApplicationBaseDirectory;

            return path.Substring(0, lastSlashIndex);
        }

        public static string GetRelativePath(string path)
        {
            var uriBase = new Uri(StaticObjects.ApplicationBaseDirectory, UriKind.Absolute);
            var uriPath = new Uri(path);

            var decodedUrl = HttpUtility.UrlDecode(uriBase.MakeRelativeUri(uriPath).ToString());
            return decodedUrl != null ? decodedUrl.Replace('/', '\\') : string.Empty;
        }

        public static string GetFileExtension(string input)
        {
            var fileName = GetPathLastPart(input);
            var firstDotIndex = fileName.IndexOf('.');
            return firstDotIndex >= 0 ? fileName.Substring(firstDotIndex) : string.Empty;
        }

        public static string GetFileNameWithoutExtension(string input)
        {
            var fileName = GetPathLastPart(input);
            var firstDotIndex = fileName.IndexOf('.');
            return firstDotIndex >= 0 ? fileName.Substring(0, firstDotIndex) : fileName;
        }

        public static object GetBoundValue(object value)
        {
            var bindingExpression = value as BindingExpression;
            if (bindingExpression != null) {
                var dataItem = bindingExpression.DataItem;
                var propertyName = bindingExpression.ParentBinding.Path.Path;

                var propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);
                return propertyValue;
            }

            return value;
        }

        public static T GetAssemblyAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(StaticObjects.ApplicationAssembly, typeof(T), false);
        }

        public static void ShowInformation(this Window window, string message)
        {
            MessageBox.Show(window, message, Properties.Resources.TextInformation, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(this Window window, string message)
        {
            MessageBox.Show(window, message, Properties.Resources.TextWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(this Window window, string message)
        {
            MessageBox.Show(window, message, Properties.Resources.TextError, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class ListBoxBehavior : DependencyObject
    {
        public static readonly DependencyProperty IsAutoScrollProperty = DependencyProperty.RegisterAttached(
            "IsAutoScrollEnabled",
            typeof(bool),
            typeof(ListBoxBehavior),
            new UIPropertyMetadata(default(bool), IsAutoScrollEnabledProperty_Changed)
        );

        public static bool GetIsAutoScrollEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAutoScrollProperty);
        }

        public static void SetIsAutoScrollEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutoScrollProperty, value);
        }

        private static void IsAutoScrollEnabledProperty_Changed(DependencyObject sender1, DependencyPropertyChangedEventArgs e1)
        {
            var listBox = sender1 as ListBox;
            Debug.Assert(listBox != null, "listBox != null");

            var itemsCollection = listBox.Items;
            var data = itemsCollection.SourceCollection as INotifyCollectionChanged;
            Debug.Assert(data != null, "data != null");

            var autoScroller = new NotifyCollectionChangedEventHandler((sender2, e2) => {
                object selectedItem = null;

                switch (e2.Action) {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Move:
                        selectedItem = e2.NewItems[e2.NewItems.Count - 1];
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if (itemsCollection.Count < e2.OldStartingIndex) {
                            selectedItem = itemsCollection[e2.OldStartingIndex - 1];
                        } else if (itemsCollection.Count > 0) {
                            selectedItem = itemsCollection[0];
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        if (itemsCollection.Count > 0) selectedItem = itemsCollection[0];
                        break;
                }

                if (selectedItem != null) {
                    itemsCollection.MoveCurrentTo(selectedItem);
                    listBox.ScrollIntoView(selectedItem);
                }
            });

            if ((bool)e1.NewValue) {
                data.CollectionChanged += autoScroller;
            } else {
                data.CollectionChanged -= autoScroller;
            }
        }
    }

    static class NativeMethods
    {
        private const int GWL_STYLE = -16,
                          WS_MAXIMIZEBOX = 0x10000,
                          WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

        public static void SetWindowButtons(this Window window, bool isMinimizable, bool isMaximizable)
        {
            var hWnd = new WindowInteropHelper(window).Handle;
            var style = GetWindowLong(hWnd, GWL_STYLE);

            if (isMaximizable) {
                style |= WS_MAXIMIZEBOX;
            } else {
                style &= ~WS_MAXIMIZEBOX;
            }

            if (isMinimizable) {
                style |= WS_MINIMIZEBOX;
            } else {
                style &= ~WS_MINIMIZEBOX;
            }

            SetWindowLong(hWnd, GWL_STYLE, style);
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        private enum ShowWindowCommands
        {
            Restore = 9
        }

        public static void RestoreWindowStateFromMinimized(this Window window)
        {
            // Restore the window's state
            if (window.WindowState == WindowState.Minimized) {
                var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
                Debug.Assert(hwndSource != null, "hwndSource != null");

                ShowWindow(hwndSource.Handle, ShowWindowCommands.Restore);
            }

            // Activate the window or its last child
            window.ActivateWindowOrLastChild();
        }
    }
}
