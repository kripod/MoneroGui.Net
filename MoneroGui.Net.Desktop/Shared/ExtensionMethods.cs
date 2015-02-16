using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;

namespace Jojatekok.MoneroGUI
{
    static class ExtensionMethods
    {
        public static void SetFormProperties(this Form form, Func<string> titleBindingPath, Size size, bool isMinimumSizeCustom = false)
        {
            form.Bind("Title", new DelegateBinding<string>(
                titleBindingPath,
                s => { form.Title = titleBindingPath.Invoke(); }
            ));

            form.Size = size;
            if (!isMinimumSizeCustom) form.MinimumSize = size;

            form.BackgroundColor = Colors.White;
        }

        public static void SetLocationToCenterScreen(this Form form)
        {
            var screenBounds = Screen.PrimaryScreen.Bounds;
            var formSize = form.Size;

            form.Location = new Point(
                (int)(screenBounds.Width / 2 - (float)formSize.Width / 2),
                (int)(screenBounds.Height / 2 - (float)formSize.Height / 2)
            );
        }

        public static void SetTextBindingPath(this TextControl textControl, Func<string> text)
        {
            textControl.Bind("Text", new DelegateBinding<string>(
                text,
                s => { textControl.Text = text.Invoke(); }
            ));
        }

        public static void SetTextBindingPath(this TabPage tabPage, Func<string> text)
        {
            tabPage.Bind("Text", new DelegateBinding<string>(
                text,
                s => { tabPage.Text = text.Invoke(); }
            ));
        }
    }
}
