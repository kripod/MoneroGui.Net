using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Jojatekok.MoneroGUI.Controls
{
    public partial class ImageTextComposition
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(ImageTextComposition)
        );

        private string _imageUri;
        public string ImageUri {
            get { return _imageUri; }

            set {
                _imageUri = value;
                Image.Source = new BitmapImage(new Uri(_imageUri, UriKind.Relative));
            }
        }

        public string Text {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public ImageTextComposition()
        {
            InitializeComponent();
        }
    }
}
