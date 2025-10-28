using System;
using System.Globalization;
using System.Windows.Data;

namespace MeteoApp.WPF.Converters
{
    // Convertit le code d'icône de l'API (ex: "04d") en une URL d'image complète
    public class MeteoIconConverter : IValueConverter
    {
        private const string BaseIconUrl = "http://openweathermap.org/img/wn/";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Vérifie si la valeur est une chaîne (le code de l'icône)
            if (value is string iconCode && !string.IsNullOrEmpty(iconCode))
            {
                return new Uri($"{BaseIconUrl}{iconCode}@2x.png");
            }

            // Retourne null si le code est manquant
            return null;
        }

        // Non utilisé pour ce cas, mais obligatoire pour IValueConverter
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}