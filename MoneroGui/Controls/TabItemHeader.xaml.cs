using System;
using System.Windows.Media.Imaging;

namespace Jojatekok.MoneroGUI.Controls
{
    public partial class TabItemHeader
    {
        private string _imageUri;
        public string ImageUri {
            get { return _imageUri; }

            set {
                _imageUri = value;
                Image.Source = new BitmapImage(new Uri(_imageUri, UriKind.Relative));
            }
        }

        public string Text {
            get { return TextBlock.Text; }
            set { TextBlock.Text = value; }
        }

        public TabItemHeader()
        {
            InitializeComponent();
        }
    }
}
