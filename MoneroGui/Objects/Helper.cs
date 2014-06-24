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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace Jojatekok.MoneroGUI
{
    static class Helper
    {
        public const string DefaultLanguageCode = "default";

        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
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

        public static string GetRelativePath(string path)
        {
            var uriBase = new Uri(StaticObjects.ApplicationDirectory, UriKind.Absolute);
            var uriPath = new Uri(path);

            var decodedUrl = HttpUtility.UrlDecode(uriBase.MakeRelativeUri(uriPath).ToString());
            return decodedUrl != null ? decodedUrl.Replace('/', '\\') : string.Empty;
        }

        public static void FocusParent(this Control control)
        {
            ((FrameworkElement)control.Parent).Focus();
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static BitmapImage ToImageSource(this Image image, bool isTransparent)
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

        public static T GetAssemblyAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(StaticObjects.ApplicationAssembly, typeof(T), false);
        }

        public static int IndexOf(this IList<SettingsManager.ConfigElementContact> collection, string label)
        {
            for (var i = collection.Count - 1; i >= 0; i--) {
                if (collection[i].Label == label) {
                    return i;
                }
            }

            return -1;
        }

        public static void ShowError(this Window window, string message)
        {
            MessageBox.Show(window, message, Properties.Resources.TextError, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class ListBoxBehavior : DependencyObject
    {
        public static bool GetIsAutoScrollEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAutoScrollProperty);
        }

        public static void SetIsAutoScrollEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutoScrollProperty, value);
        }

        public static readonly DependencyProperty IsAutoScrollProperty = DependencyProperty.RegisterAttached(
            "IsAutoScrollEnabled",
            typeof(bool),
            typeof(ListBoxBehavior),
            new UIPropertyMetadata(default(bool), IsAutoScrollEnabledProperty_Changed)
        );

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
    }
}
