using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public static ImageSource ToImageSource(this Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static T GetAssemblyAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(StaticObjects.ApplicationAssembly, typeof(T), false);
        }

        public static string GetRelativePath(string path)
        {
            var uriBase = new Uri(StaticObjects.ApplicationDirectory, UriKind.Absolute);
            var uriPath = new Uri(path);
            return uriBase.MakeRelativeUri(uriPath).ToString().Replace("%20", " ").Replace('/', '\\');
        }
    }

    static class NativeMethods
    {
        private const int GWL_STYLE = -16,
                          WS_MAXIMIZEBOX = 0x10000,
                          WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        extern private static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        extern private static int SetWindowLong(IntPtr hwnd, int index, int value);

        public static void SetWindowButtons(this Window window, bool isMinimizable, bool isMaximizable)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var style = GetWindowLong(hwnd, GWL_STYLE);

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

            SetWindowLong(hwnd, GWL_STYLE, style);
        }
    }
}
