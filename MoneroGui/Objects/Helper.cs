using Jojatekok.MoneroGUI.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string EncodeUrl(string value)
        {
            if (value == null) {
                return null;
            }

            var length = value.Length;
            var stringBuilder = new StringBuilder(length);

            for (var i = 0; i < length; i++) {
                var ch = value[i];

                if ((ch & 0xff80) == 0) {
                    if (IsUrlSafeChar(ch)) {
                        stringBuilder.Append(ch);
                    } else if (ch == ' ') {
                        stringBuilder.Append("%20");
                    } else {
                        stringBuilder.Append(
                            "%" +
                            IntToHex((ch >> 4) & 0xf) +
                            IntToHex((ch) & 0xf)
                        );
                    }

                } else {
                    // Unicode character encoding
                    stringBuilder.Append(
                        "%u" +
                        IntToHex((ch >> 12) & 0xf) +
                        IntToHex((ch >> 8) & 0xf) +
                        IntToHex((ch >> 4) & 0xf) +
                        IntToHex((ch) & 0xf)
                    );
                }
            }

            return stringBuilder.ToString();
        }

        internal static string DecodeUrl(string value)
        {
            if (value == null) {
                return null;
            }

            var length = value.Length;
            var stringBuilder = new StringBuilder(length);

            // Go through the string's chars collapsing %XX and %uXXXX, and appending each 
            // char as char, with exception of %XX constructs that are appended as bytes

            for (var i = 0; i < length; i++) {
                var ch = value[i];

                if (ch == '%' && i < length - 2) {
                    if (value[i + 1] == 'u' && i < length - 5) {
                        var h1 = HexToInt(value[i + 2]);
                        var h2 = HexToInt(value[i + 3]);
                        var h3 = HexToInt(value[i + 4]);
                        var h4 = HexToInt(value[i + 5]);

                        if (h1 >= 0 && h2 >= 0 && h3 >= 0 && h4 >= 0) {
                            // Valid 4 hexadecimal chars
                            i += 5;
                            stringBuilder.Append((char)((h1 << 12) | (h2 << 8) | (h3 << 4) | h4));
                            continue;
                        }

                    } else {
                        var h1 = HexToInt(value[i + 1]);
                        var h2 = HexToInt(value[i + 2]);

                        if (h1 >= 0 && h2 >= 0) {
                            // Valid 2 hexadecimal chars
                            i += 2;
                            stringBuilder.Append((char)((h1 << 4) | h2));
                            continue;
                        }
                    }
                }

                stringBuilder.Append(ch);
            }

            return stringBuilder.ToString();
        }

        private static bool IsUrlSafeChar(char input) {
            if ((input >= 'a' && input <= 'z') || (input >= 'A' && input <= 'Z') || (input >= '0' && input <= '9')) {
                return true;
            }

            switch (input) {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

        private static char IntToHex(int input) {
            if (input <= 9) return (char)(input + '0');
            return (char)(input - 10 + 'a');
        }

        public static int HexToInt(char h) {
            return (h >= '0' && h <= '9') ? h - '0' :
            (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
            (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
            -1;
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

            var decodedUrl = DecodeUrl(uriBase.MakeRelativeUri(uriPath).ToString());
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
            MessageBoxEx.Show(window, Properties.Resources.TextInformation, message, SystemIcons.Information, Properties.Resources.TextOk);
        }

        public static byte ShowQuestion(this Window window, string message, string title)
        {
            return MessageBoxEx.Show(window, title, message, SystemIcons.Question, Properties.Resources.TextYes, Properties.Resources.TextNo);
        }

        public static void ShowWarning(this Window window, string message)
        {
            MessageBoxEx.Show(window, Properties.Resources.TextWarning, message, SystemIcons.Warning, Properties.Resources.TextOk);
        }

        public static void ShowError(this Window window, string message)
        {
            MessageBoxEx.Show(window, Properties.Resources.TextError, message, SystemIcons.Error, Properties.Resources.TextOk);
        }
    }

    static class NativeMethods
    {
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        public enum ShowWindowCommands
        {
            Restore = 9
        }

        public static void RestoreWindowStateFromMinimized(this Window window)
        {
            // Restore the window's state
            if (window.WindowState == WindowState.Minimized) {
                ShowWindow(new WindowInteropHelper(window).Handle, ShowWindowCommands.Restore);
            }

            // Activate the window or its last child
            window.ActivateWindowOrLastChild();
        }

        private const int GWL_STYLE = -16,
                          WS_MAXIMIZEBOX = 0x10000,
                          WS_MINIMIZEBOX = 0x20000;

        [DllImport("User32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("User32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int index, int value);

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
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIdEnableItem, uint uEnable);

        const uint MF_BYCOMMAND = 0x00000000;
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;

        const uint SC_CLOSE = 0xF060;

        public static void SetWindowButtonClose(this Window window, bool isEnabled)
        {
            var hMenu = GetSystemMenu(new WindowInteropHelper(window).Handle, false);
            if (hMenu != IntPtr.Zero) {
                if (isEnabled) {
                    EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
                } else {
                    EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
                }
            }
        }
    }
}
