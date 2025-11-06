using System;
using System.Globalization;
using System.Windows.Data;

namespace MeteoApp.WPF.Converters
{
    // Convertit un booléen en son inverse (True -> False / False -> True)
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                // Inverse la valeur pour les propriétés IsEnabled
                return !booleanValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}