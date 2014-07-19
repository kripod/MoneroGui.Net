using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    public class ConverterBooleanToString : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.RegisterAttached(
            "FalseValue",
            typeof(string),
            typeof(ConverterBooleanToString)
        );

        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.RegisterAttached(
            "TrueValue",
            typeof(string),
            typeof(ConverterBooleanToString)
        );

        public string FalseValue {
            private get { return GetValue(FalseValueProperty) as string; }
            set { SetValue(FalseValueProperty, value); }
        }

        public string TrueValue {
            private get { return GetValue(TrueValueProperty) as string; }
            set { SetValue(TrueValueProperty, value); }
        }

        private static ConverterBooleanToString _provider;
        public static ConverterBooleanToString Provider {
            get {
                if (_provider == null) {
                    _provider = Application.Current.FindResource("ConverterBooleanToStringYesNo") as ConverterBooleanToString;
                }

                return _provider;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return FalseValue;
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue);
        }
    }
}
