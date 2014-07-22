using System;
using System.Globalization;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    public class ConverterNullableUlongToUriPartString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as ulong?;
            if (input == null || input == 0) return null;

            return parameter + "=" + Helper.EncodeUrl(input.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as ulong?;
            if (input == null || input == 0) return null;

            return Helper.DecodeUrl(input.ToString());
        }
    }
}
