using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MeteoApp.WPF.Converters
{
    // Convertit un booléen en une chaîne de caractères (TrueValue ou FalseValue)
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue && parameter is string parameterString)
            {
                // Le paramètre doit être au format "ValeurSiVrai|ValeurSiFaux"
                string[] texts = parameterString.Split('|');

                if (texts.Length == 2)
                {
                    // Si True, retourne le premier élément (ex: "Chargement")
                    return booleanValue ? texts[0] : texts[1];
                }
            }

            // Retourne une chaîne vide si la conversion échoue
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Conversion inverse non requise
            return DependencyProperty.UnsetValue;
        }
    }
}