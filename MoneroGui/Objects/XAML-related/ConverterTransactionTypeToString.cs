using Jojatekok.MoneroAPI;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    public class ConverterTransactionTypeToString : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty UnknownValueProperty = DependencyProperty.RegisterAttached(
            "UnknownValue",
            typeof(string),
            typeof(ConverterTransactionTypeToString)
        );

        public string UnknownValue {
            private get { return GetValue(UnknownValueProperty) as string; }
            set { SetValue(UnknownValueProperty, value); }
        }

        public static readonly DependencyProperty ReceiveValueProperty = DependencyProperty.RegisterAttached(
            "ReceiveValue",
            typeof(string),
            typeof(ConverterTransactionTypeToString)
        );

        public string ReceiveValue {
            private get { return GetValue(ReceiveValueProperty) as string; }
            set { SetValue(ReceiveValueProperty, value); }
        }

        public static readonly DependencyProperty SendValueProperty = DependencyProperty.RegisterAttached(
            "SendValue",
            typeof(string),
            typeof(ConverterTransactionTypeToString)
        );

        public string SendValue {
            private get { return GetValue(SendValueProperty) as string; }
            set { SetValue(SendValueProperty, value); }
        }

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
