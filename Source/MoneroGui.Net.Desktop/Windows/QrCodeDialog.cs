using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class QrCodeDialog : Dialog, INotifyPropertyChanged
    {
        private const int QrCodeImageSize = 330;

        private readonly Panel _panelQrCode = new Panel { Width = QrCodeImageSize, Height = QrCodeImageSize };
        private readonly ImageView _imageViewQrCode = new ImageView { Width = QrCodeImageSize, Height = QrCodeImageSize };
        private readonly Label _labelQrUriError = new Label { TextAlignment = TextAlignment.Center };
        private readonly Button _buttonSaveAs = new Button { Text = Desktop.Properties.Resources.TextSaveAs, Image = Utilities.LoadImage("Save") };

        private string _qrUri = "";
        private string _address = "";
        private string _paymentId = "";
        private double _amount;
        private string _label = "";
        private string _message = "";

        private Panel PanelQrCode {
            get { return _panelQrCode; }
        }

        private ImageView ImageViewQrCode {
            get { return _imageViewQrCode; }
        }

        private Label LabelQrUriError {
            get { return _labelQrUriError; }
        }

        private Button ButtonSaveAs {
            get { return _buttonSaveAs; }
        }

        public string QrUri {
            get { return _qrUri; }
            set {
                _qrUri = value;
                OnPropertyChanged();
                if (Loaded) UpdateQrCodeImageTask().Start();
            }
        }

        public string Address {
            get { return _address; }
            set {
                _address = value;
                OnPropertyChanged();
                if (Loaded) RefreshQrUri();
            }
        }

        public string PaymentId {
            get { return _paymentId; }
            set {
                _paymentId = value;
                OnPropertyChanged();
                if (Loaded) RefreshQrUri();
            }
        }

        public double Amount {
            get { return _amount; }
            set {
                _amount = value;
                OnPropertyChanged();
                if (Loaded) RefreshQrUri();
            }
        }

        public string Label {
            get { return _label; }
            set {
                _label = value;
                OnPropertyChanged();
                if (Loaded) RefreshQrUri();
            }
        }

        public string Message {
            get { return _message; }
            set {
                _message = value;
                OnPropertyChanged();
                if (Loaded) RefreshQrUri();
            }
        }

        private string QrUriErrorMessage {
            set {
                if (value == LabelQrUriError.Text) return;
                LabelQrUriError.Text = value;

                if (value.Length > 0) {
                    // There is an error
                    PanelQrCode.Content = LabelQrUriError;
                    ButtonSaveAs.Enabled = false;

                } else {
                    // There are no errors
                    PanelQrCode.Content = ImageViewQrCode;
                    ButtonSaveAs.Enabled = true;
                }
            }
        }

        public QrCodeDialog(SettingsManager.ConfigElementContact contact)
        {
            this.SetWindowProperties(
                Desktop.Properties.Resources.TextQrCode,
                new Size(0, 0)
            );

            Address = contact.Address;
            Label = contact.Label;

            RenderContent();
        }

        public QrCodeDialog(string address = "") : this(new SettingsManager.ConfigElementContact("", address))
        {
            
        }

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            PanelQrCode.Content = ImageViewQrCode;
            ButtonSaveAs.Click += OnButtonSaveAsClick;

            var textBoxLabel = Utilities.CreateTextBox(this, o => o.Label);
            textBoxLabel.PlaceholderText = Label;

            var textBoxQrUri = Utilities.CreateTextBox(this, o => o.QrUri);
            textBoxQrUri.ReadOnly = true;

            var buttonCopyQrUri = new Button {
                Image = Utilities.LoadImage("Copy"),
                ToolTip = Desktop.Properties.Resources.TextCopy
            };
            buttonCopyQrUri.Click += delegate { Utilities.Clipboard.Text = QrUri; };

            DefaultButton = ButtonSaveAs;

            Content = new TableLayout(
                PanelQrCode,

                new TableRow(
                    new TableLayout(
                        new TableRow(
                            new TableCell(textBoxQrUri, true),
                            buttonCopyQrUri
                        )
                    ) { Spacing = Utilities.Spacing3 }
                ),

                new TableRow(
                    new TableLayout(
                        new TableRow(
                            new Label { Text = Desktop.Properties.Resources.TextAddress + Desktop.Properties.Resources.PunctuationColon },
                            new TableCell(Utilities.CreateTextBox(this, o => o.Address), true)
                        ),

                        new TableRow(
                            new Label { Text = Desktop.Properties.Resources.TextPaymentId },
                            new TableCell(Utilities.CreateTextBox(this, o => o.PaymentId), true)
                        ),

                        new TableRow(
                            new Label { Text = Desktop.Properties.Resources.TextAmount + Desktop.Properties.Resources.PunctuationColon },
                            new TableLayout(
                                new TableRow(
                                    new TableCell(Utilities.CreateNumericUpDown(this, o => o.Amount), true),
                                    new Label { Text = Desktop.Properties.Resources.TextCurrencyCode }
                                )
                            ) { Spacing = Utilities.Spacing3 }
                        ),

                        new TableRow(
                            new Label { Text = Desktop.Properties.Resources.TextLabel + Desktop.Properties.Resources.PunctuationColon },
                            new TableCell(textBoxLabel, true)
                        ),

                        new TableRow(
                            new Label { Text = Desktop.Properties.Resources.TextMessage },
                            new TableCell(Utilities.CreateTextBox(this, o => o.Message), true)
                        )
                    ) { Padding = new Padding(0, Utilities.Padding3), Spacing = Utilities.Spacing3 }
                ),

                new TableRow(
                    new TableRow(
                        new TableCell { ScaleWidth = true },
                        ButtonSaveAs
                    )
                ),

                new TableRow()
            );

            RefreshQrUri();
            UpdateQrCodeImageTask().RunSynchronously();
        }

        void RefreshQrUri()
        {
            if (Address.Length == 0) {
                QrUri = "";
                return;
            }

            var output = PaymentUriParameters.ProtocolPreTag + ":" + Address;

            var parameters = new HashSet<string>();

            if (Message.Length != 0) parameters.Add(PaymentUriParameters.Message + "=" + Message);
            if (PaymentId.Length != 0) parameters.Add(PaymentUriParameters.PaymentId + "=" + PaymentId);
            if (Amount > 0) parameters.Add(PaymentUriParameters.Amount + "=" + MoneroAPI.Utilities.CoinDisplayValueToAtomicValue(Amount));
            if (Label.Length != 0) parameters.Add(PaymentUriParameters.Label + "=" + Label);

            if (parameters.Count != 0) {
                output += "?" + string.Join("&", parameters);
            }

            QrUri = output;
        }

        Task UpdateQrCodeImageTask()
        {
            if (QrUri.Length == 0) {
                // Notify user about the missing address parameter
                return new Task(() => Application.Instance.AsyncInvoke(() => QrUriErrorMessage = Desktop.Properties.Resources.QrCodeWindowUriNoAddress));
            }

            if (QrUri.Length > 1024) {
                // Notify user about the QR URI's size being too big
                return new Task(() => Application.Instance.AsyncInvoke(() => QrUriErrorMessage = Desktop.Properties.Resources.QrCodeWindowUriTooLong));
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

                using (var bitmap = writer.Write(QrUri)) {
// ReSharper disable once AccessToDisposedClosure
                    Application.Instance.Invoke(() => SetQrCodeImageSource(bitmap));
                }
            });
        }

        void SetQrCodeImageSource(System.Drawing.Bitmap bitmap)
        {
            ImageViewQrCode.Image = new Bitmap(Utilities.SystemImageConverter.ConvertTo(bitmap, typeof(byte[])) as byte[]);
            bitmap.Dispose();

            QrUriErrorMessage = string.Empty;
        }

        void OnButtonSaveAsClick(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Directory = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)) }) {
                dialog.Filters.Add(new FileDialogFilter(Desktop.Properties.Resources.TextFilterPngFiles, Utilities.FileFilterPng));
                dialog.Filters.Add(new FileDialogFilter(Desktop.Properties.Resources.TextFilterAllFiles, Utilities.FileFilterAll));

                if (dialog.ShowDialog(this) != DialogResult.Ok) return;

                var fileName = dialog.FileName;
                Task.Factory.StartNew(() => SaveQrCodeImage(ImageViewQrCode.Image, fileName));
            }
        }

        private static void SaveQrCodeImage(Image image, string fileName)
        {
            Debug.Assert(image as Bitmap != null, "image as Bitmap != null");
            using (var memoryStream = new MemoryStream((image as Bitmap).ToByteArray(ImageFormat.Png))) {
                using (var bitmap = new System.Drawing.Bitmap(memoryStream)) {
                    bitmap.MakeTransparent(System.Drawing.Color.White);

                    using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096)) {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
