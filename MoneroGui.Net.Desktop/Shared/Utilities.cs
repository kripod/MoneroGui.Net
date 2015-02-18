using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using System.Reflection;
using System.Threading;

namespace Jojatekok.MoneroGUI
{
    struct Utilities
    {
        public static readonly Color ColorSeparator = Color.FromRgb(10526880);
        public static readonly Color ColorStatusBar = Color.FromRgb(15855085);

        public const byte FontSizeTitle = 12;

        public const byte PaddingExtraSmall = 3;
        public const byte PaddingSmall = 5;
        public const byte PaddingMedium = 8;
        public const byte PaddingLarge = 10;
        public const byte PaddingExtraLarge = 20;

        public static readonly BindingCollection BindingsToAccountBalance = new BindingCollection();

        public static readonly Assembly ApplicationAssembly = Assembly.GetExecutingAssembly();
        public static readonly AssemblyName ApplicationAssemblyName = ApplicationAssembly.GetName();
        public static readonly string ApplicationAssemblyNameName = ApplicationAssembly.GetName().Name;

        public static readonly Version ApplicationVersionComparable = ApplicationAssemblyName.Version;
        public const string ApplicationVersionExtra = null;
        public static readonly string ApplicationVersionString = ApplicationVersionComparable.ToString(3) + (ApplicationVersionExtra != null ? "-" + ApplicationVersionExtra : null);

        public static SynchronizationContext SyncContextMain { get; set; }

        public static readonly Properties.Resources ResourcesInstance = new Properties.Resources();

        private static readonly ImageConverter ImageConverter = new ImageConverter();

        public static void Initialize()
        {
            SyncContextMain = SynchronizationContext.Current;
		    using (var button = new Button()) {
		        var handler = button.Handler;

                var fieldInfo = handler.GetType().GetField("MinimumSize");
		        if (fieldInfo != null) {
		            var size = (Size)(fieldInfo.GetValue(null));
		            size.Width = 0;
                    fieldInfo.SetValue(null, size);

		        } else {
                    fieldInfo = handler.GetType().GetField("MinimumWidth");
		            if (fieldInfo != null) {
		                fieldInfo.SetValue(null, 0);
		            }
		        }
		    }
        }

        public static Image LoadImage(string resourceName)
        {
            return ImageConverter.ConvertFrom(
                ImageConverter.ResourcePrefix +
                "Jojatekok.MoneroGUI." + resourceName + ".png," +
                ApplicationAssemblyNameName
            ) as Image;
        }

        public static Label CreateLabel(Func<string> textBinding, VerticalAlign verticalAlignment = VerticalAlign.Middle, Font font = null)
        {
            var label = new Label {
                VerticalAlign = verticalAlignment
            };

            label.SetTextBindingPath(textBinding);
            if (font != null) label.Font = font;

            return label;
        }

        public static TextBox CreateTextBox(Func<string> placeholderTextBinding, string text = null, Font font = null)
        {
            var textBox = new TextBox {
                Text = text
            };

            textBox.SetPlaceholderTextBindingPath(placeholderTextBinding);
            if (font != null) textBox.Font = font;

            return textBox;
        }

        public static Button CreateButton(Func<string> textBinding, Image image = null)
        {
            var button = new Button {
                Image = image
            };

            button.SetTextBindingPath(textBinding);

            return button;
        }

        public static NumericUpDown CreateNumericUpDown()
        {
            var numericUpDown = new NumericUpDown {
                 
            };

            return numericUpDown;
        }
    }
}
