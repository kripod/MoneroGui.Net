using Jojatekok.MoneroAPI;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    public class ConverterTransactionTypeToString : IValueConverter
    {
        public string UnknownValue { get; set; }
        public string ReceiveValue { get; set; }
        public string SendValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return UnknownValue;

            switch ((TransactionType)value) {
                case TransactionType.Receive:
                    return ReceiveValue;

                case TransactionType.Send:
                    return SendValue;

                default:
                    return UnknownValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueString = value as string;

            if (valueString == ReceiveValue) return TransactionType.Receive;
            if (valueString == SendValue) return TransactionType.Send;
            return TransactionType.Unknown;
        }
    }
}
