using System;
using System.Globalization;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    [ValueConversion(typeof(ulong), typeof(double))]
    public class ConverterCoinAtomicValueToDisplayValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return (ulong)value / StaticObjects.CoinAtomicValueDivider;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return (ulong)Math.Round((double)value * StaticObjects.CoinAtomicValueDivider);
        }
    }
}
