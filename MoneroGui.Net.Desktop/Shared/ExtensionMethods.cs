using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Eto.IO;

namespace Jojatekok.MoneroGUI
{
    static class ExtensionMethods
    {
        public static void SetWindowProperties(this Form form, Func<string> titleBindingPath, Size size, bool isMinimumSizeCustom = false)
        {
            form.Bind("Title", new DelegateBinding<string>(
                titleBindingPath,
                s => { form.Title = titleBindingPath.Invoke(); }
            ));

            form.Size = size;
            if (!isMinimumSizeCustom) form.MinimumSize = size;

            form.BackgroundColor = Colors.White;
        }

        public static void SetWindowProperties(this Dialog dialog, string title, Size minimumSize = default(Size))
        {
            dialog.Title = title;
            dialog.MinimumSize = minimumSize;

            dialog.BackgroundColor = Colors.White;
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

        public static void ShowInformation(this Control control, string message)
        {
            MessageBox.Show(control, message, Properties.Resources.TextInformation);
        }

        public static bool ShowQuestion(this Control control, string message, string title, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Yes)
        {
            return MessageBox.Show(control, message, title, MessageBoxButtons.YesNo, MessageBoxType.Question, defaultButton) == DialogResult.Yes;
        }

        public static void ShowWarning(this Control control, string message)
        {
            MessageBox.Show(control, message, Properties.Resources.TextWarning, MessageBoxType.Warning);
        }

        public static void ShowError(this Control control, string message)
        {
            MessageBox.Show(control, message, Properties.Resources.TextError, MessageBoxType.Error);
        }
    }
}
