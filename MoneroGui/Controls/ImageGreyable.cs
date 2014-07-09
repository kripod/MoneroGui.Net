using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jojatekok.MoneroGUI.Controls
{
    class ImageGreyable : Image
    {
        static ImageGreyable()
        {
#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
#endif

            IsEnabledProperty.OverrideMetadata(
                typeof(ImageGreyable),
                new FrameworkPropertyMetadata(true, IsEnabledProperty_Changed)
            );
        }

        private static void IsEnabledProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var image = sender as ImageGreyable;
            if (image == null || image.Source == null) return;

            // Check whether the image is enabled
            if ((bool)e.NewValue) {
                image.Source = ((FormatConvertedBitmap)image.Source).Source;
                image.OpacityMask = null;

            } else {
                var bitmapImage = new BitmapImage(new Uri(image.Source.ToString(Helper.InvariantCulture)));
                image.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);
                image.OpacityMask = new ImageBrush(bitmapImage);
            }
        }
    }
}
