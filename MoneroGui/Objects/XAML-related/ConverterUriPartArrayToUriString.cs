using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    public class ConverterUriPartArrayToUriString : IMultiValueConverter
    {
        private static ConverterUriPartArrayToUriString _provider;
        public static ConverterUriPartArrayToUriString Provider {
            get {
                if (_provider == null) {
                    _provider = Application.Current.FindResource("ConverterUriPartArrayToUriString") as ConverterUriPartArrayToUriString;
                }

                return _provider;
            }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var output = values[0] as string;
            if (string.IsNullOrEmpty(output)) return null;

            var isCurrentValueFirst = true;
            for (var i = 1; i < values.Length; i++) {
                var value = values[i] as string;
                if (string.IsNullOrEmpty(value)) continue;

                if (isCurrentValueFirst) {
                    isCurrentValueFirst = false;
                    output += "?" + value;
                } else {
                    output += "&" + value;
                }
            }

            return QrUriParameters.ProtocolPreTag + output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var input = value as string;
            if (string.IsNullOrEmpty(input)) return null;

// ReSharper disable once CoVariantArrayConversion
            return input.Split('?', '&');
        }
    }
}
