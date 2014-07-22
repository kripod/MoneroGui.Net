using System;
using System.Globalization;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    public class ConverterStringToUriPartString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as string;
            if (string.IsNullOrEmpty(input)) return null;

            return parameter + "=" + Helper.EncodeUrl(input);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as string;
            if (string.IsNullOrEmpty(input)) return null;

            return Helper.DecodeUrl(input);
        }
    }
}
