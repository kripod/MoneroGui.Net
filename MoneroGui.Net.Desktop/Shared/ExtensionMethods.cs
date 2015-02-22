using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using System.Text.RegularExpressions;

namespace Jojatekok.MoneroGUI
{
    static class ExtensionMethods
    {
        public static void SetWindowProperties(this Window window, Func<string> titleBindingPath, Size size, bool isMinimumSizeCustom = false)
        {
            window.Bind("Title", new DelegateBinding<string>(
                titleBindingPath,
                s => { window.Title = titleBindingPath.Invoke(); }
            ));

            window.Size = size;
            if (!isMinimumSizeCustom) window.MinimumSize = size;

            window.BackgroundColor = Colors.White;
        }

        public static void SetLocationToCenterScreen(this Window window)
        {
            var screenBounds = Screen.PrimaryScreen.Bounds;
            var windowSize = window.Size;

            window.Location = new Point(
                (int)(screenBounds.Width / 2 - (float)windowSize.Width / 2),
                (int)(screenBounds.Height / 2 - (float)windowSize.Height / 2)
            );
        }

        public static void SetTextBindingPath(this TextControl textControl, Func<string> textBinding)
        {
            textControl.Bind("Text", new DelegateBinding<string>(
                textBinding,
                s => { textControl.Text = textBinding.Invoke(); }
            ));
        }

        public static void SetTextBindingPath(this TabPage tabPage, Func<string> textBinding)
        {
            tabPage.Bind("Text", new DelegateBinding<string>(
                textBinding,
                s => { tabPage.Text = textBinding.Invoke(); }
            ));
        }

        public static void SetPlaceholderTextBindingPath(this TextBox textBox, Func<string> placeholderTextBinding)
        {
            textBox.Bind("PlaceholderText", new DelegateBinding<string>(
                placeholderTextBinding,
                s => { textBox.PlaceholderText = placeholderTextBinding.Invoke(); }
            ));
        }

        public static string ReWrap(this string input)
        {
            return Regex.Replace(input.TrimEnd(), " (\r\n|\n)", " ");
        }
    }
}
