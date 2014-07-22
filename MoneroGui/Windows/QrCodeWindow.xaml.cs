using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using Color = System.Drawing.Color;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class QrCodeWindow
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached(
            "ImageSource",
            typeof(BitmapImage),
            typeof(QrCodeWindow)
        );

        public static readonly DependencyProperty QrDataVisibilityProperty = DependencyProperty.RegisterAttached(
            "QrDataVisibility",
            typeof(Visibility),
            typeof(QrCodeWindow)
        );

        public static readonly DependencyProperty AddressProperty = DependencyProperty.RegisterAttached(
            "Address",
            typeof(string),
            typeof(QrCodeWindow)
        );

        public static readonly DependencyProperty PaymentIdProperty = DependencyProperty.RegisterAttached(
            "PaymentId",
            typeof(string),
            typeof(QrCodeWindow)
        );

        public static readonly DependencyProperty AmountProperty = DependencyProperty.RegisterAttached(
            "Amount",
            typeof(ulong?),
            typeof(QrCodeWindow)
        );

        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
            "Label",
            typeof(string),
            typeof(QrCodeWindow)
        );

        public static readonly DependencyProperty MessageProperty = DependencyProperty.RegisterAttached(
            "Message",
            typeof(string),
            typeof(QrCodeWindow)
        );

        public BitmapImage ImageSource {
            get { return GetValue(ImageSourceProperty) as BitmapImage; }
            private set { SetValue(ImageSourceProperty, value); }
        }

        public Visibility QrDataVisibility {
            get { return (Visibility)GetValue(QrDataVisibilityProperty); }
            private set { SetValue(QrDataVisibilityProperty, value); }
        }

        public string Address {
            get { return GetValue(AddressProperty) as string; }
            set { SetValue(AddressProperty, value); }
        }

        public string PaymentId {
            get { return GetValue(PaymentIdProperty) as string; }
            set { SetValue(PaymentIdProperty, value); }
        }

        public ulong? Amount {
            get { return GetValue(AmountProperty) as ulong?; }
            set { SetValue(AmountProperty, value); }
        }

        public string Label {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }

        public string Message {
            get { return GetValue(MessageProperty) as string; }
            set { SetValue(MessageProperty, value); }
        }

        private string UriErrorMessage {
            set {
                if (value == TextBlockUriError.Text) return;
                TextBlockUriError.Text = value;

                if (value != string.Empty) {
                    // There is an error
                    ImageQrCode.Visibility = Visibility.Hidden;
                    TextBlockUriError.Visibility = Visibility.Visible;
                    ButtonSaveAs.IsEnabled = false;

                } else {
                    // There are no errors
                    TextBlockUriError.Visibility = Visibility.Collapsed;
                    ImageQrCode.Visibility = Visibility.Visible;
                    ButtonSaveAs.IsEnabled = true;
                }
            }
        }

        public QrCodeWindow(Window owner, SettingsManager.ConfigElementContact contact)
        {
            Icon = StaticObjects.ApplicationIconImage;
            SourceInitialized += delegate {
                this.SetWindowButtons(false, true);

                MinWidth = ActualWidth;
                MinHeight = ActualHeight;
            };

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

        private void ImageQrCode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Check only for double clicks
            if (e.ClickCount != 2) return;

            if (QrDataVisibility == Visibility.Visible) {
                QrDataVisibility = Visibility.Collapsed;
                ImageQrCode.ToolTip = Properties.Resources.QrCodeWindowImageShrink;

            } else {
                QrDataVisibility = Visibility.Visible;
                ImageQrCode.ToolTip = Properties.Resources.QrCodeWindowImageEnlarge;
            }
        }

        private void TextBoxQrUri_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            TextBoxQrUri.SelectAll();
        }

        private void TextBoxQrUri_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded) UpdateQrCodeImageTask().Start();
        }

        private Task UpdateQrCodeImageTask()
        {
            var qrUriText = Dispatcher.Invoke(() => TextBoxQrUri.Text);
            if (qrUriText.Length == 0) {
                // Notify user about the missing address parameter
                return new Task(() => Dispatcher.BeginInvoke(new Action(() => UriErrorMessage = Properties.Resources.QrCodeWindowUriNoAddress), DispatcherPriority.DataBind));
            }

            if (qrUriText.Length > 1024) {
                // Notify user about the QR URI's size being too big
                return new Task(() => Dispatcher.BeginInvoke(new Action(() => UriErrorMessage = Properties.Resources.QrCodeWindowUriTooLong), DispatcherPriority.DataBind));
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
                        UriErrorMessage = string.Empty;
                    }, DispatcherPriority.DataBind);
                }
            });
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
    }
}
