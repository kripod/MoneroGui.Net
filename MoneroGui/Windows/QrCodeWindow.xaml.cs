using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using Color = System.Drawing.Color;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class QrCodeWindow : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached(
            "ImageSource",
            typeof(BitmapImage),
            typeof(QrCodeWindow)
        );

        public BitmapImage ImageSource {
            get { return GetValue(ImageSourceProperty) as BitmapImage; }
            set { SetValue(ImageSourceProperty, value); }
        }

        private string _address;
        public string Address {
            get { return _address; }

            private set {
                _address = value;
                OnPropertyChanged();
            }
        }

        private string _label;
        public string Label {
            get { return _label; }

            set {
                _label = value;
                OnPropertyChanged();

                if (value.Length != 0) {
                    LabelUriPart = "&label=" + Uri.EscapeUriString(value);
                } else {
                    LabelUriPart = null;
                }
            }
        }

        private string _labelUriPart;
        public string LabelUriPart {
            get { return _labelUriPart; }

            private set {
                _labelUriPart = value;
                OnPropertyChanged();
            }
        }

        public string Message {
            set {
                if (value.Length != 0) {
                    MessageUriPart = "&message=" + Uri.EscapeUriString(value);
                } else {
                    MessageUriPart = null;
                }
            }
        }

        private string _messageUriPart;
        public string MessageUriPart {
            get { return _messageUriPart; }

            private set {
                _messageUriPart = value;
                OnPropertyChanged();
            }
        }

        public string PaymentId {
            set {
                if (value.Length != 0) {
                    PaymentIdUriPart = "&paymentId=" + Uri.EscapeUriString(value);
                } else {
                    PaymentIdUriPart = null;
                }
            }
        }

        private string _paymentIdUriPart;
        public string PaymentIdUriPart {
            get { return _paymentIdUriPart; }

            private set {
                _paymentIdUriPart = value;
                OnPropertyChanged();
            }
        }

        public QrCodeWindow(Window owner, SettingsManager.ConfigElementContact contact)
        {
            Icon = StaticObjects.ApplicationIconImage;

            InitializeComponent();

            Owner = owner;
            Address = contact.Address;
            Label = contact.Label;
            TextBoxLabel.Watermark = contact.Label;

            UpdateQrCodeImageTask().RunSynchronously();
        }

        public QrCodeWindow(Window owner, string address) : this(owner, new SettingsManager.ConfigElementContact(string.Empty, address))
        {

        }

        private bool IsQrUriTooLong {
            set {
                if (value) {
                    ImageQrCode.Visibility = Visibility.Hidden;
                    TextBlockUriIsTooLong.Visibility = Visibility.Visible;

                } else {
                    TextBlockUriIsTooLong.Visibility = Visibility.Hidden;
                    ImageQrCode.Visibility = Visibility.Visible;
                }

                ButtonSaveAs.IsEnabled = !value;
            }
        }

        private void TextBoxQrUri_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded) UpdateQrCodeImageTask().Start();
        }

        private Task UpdateQrCodeImageTask()
        {
            var qrUriText = Dispatcher.Invoke(() => TextBoxQrUri.Text);
            if (qrUriText.Length > 1024) {
                // Notify user about the QR URI's size being too big
                return new Task(() => Dispatcher.Invoke(() => IsQrUriTooLong = true ));
            }

            return new Task(() => {
                var writer = new BarcodeWriter {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions {
                        ErrorCorrection = ErrorCorrectionLevel.H,
                        Width = 512,
                        Height = 512
                    }
                };

                using (var bitmap = new Bitmap(writer.Write(qrUriText))) {
                    Dispatcher.Invoke(() => {
// ReSharper disable once AccessToDisposedClosure
                        ImageSource = bitmap.ToBitmapImage(false);
                        IsQrUriTooLong = false;
                    });
                }
            });
        }

        private void TextBoxQrUri_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBoxQrUri.SelectAll();
        }

        private void ButtonCopyQrUri_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBoxQrUri.Text);

            TextBoxQrUri.SelectAll();
            this.SetFocusedElement(TextBoxQrUri);
        }

        private void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog {
                Filter = Properties.Resources.TextFilterPngFiles + "|" + Properties.Resources.TextFilterAllFiles,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (dialog.ShowDialog() != true) return;

            Task.Factory.StartNew(() => SaveQrCodeImage(Dispatcher.Invoke(() => ImageSource), dialog.FileName));
        }

        private void SaveQrCodeImage(BitmapSource image, string fileName)
        {
            // Make sure the latest version of the QR code is used
            UpdateQrCodeImageTask().RunSynchronously();

            var formatConvertedBitmap = new FormatConvertedBitmap();
            formatConvertedBitmap.BeginInit();

            using (var bitmap = image.ToBitmap()) {
                bitmap.MakeTransparent(Color.White);
                formatConvertedBitmap.Source = bitmap.ToBitmapImage(true);
            }

            formatConvertedBitmap.DestinationFormat = PixelFormats.Default;
            formatConvertedBitmap.EndInit();

            var encoder = new PngBitmapEncoder { Interlace = PngInterlaceOption.On };
            encoder.Frames.Add(BitmapFrame.Create(formatConvertedBitmap));

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096)) {
                encoder.Save(stream);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
