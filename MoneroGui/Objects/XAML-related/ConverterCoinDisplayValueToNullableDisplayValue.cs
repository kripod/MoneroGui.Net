using System;
using System.Globalization;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    [ValueConversion(typeof(double), typeof(object))]
    public class ConverterCoinDisplayValueToNullableDisplayValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? Properties.Resources.PunctuationQuestionMark;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as string == null ? value : null;
        }
    }
}
