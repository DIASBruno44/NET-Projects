using System;
using System.Globalization;
using System.Windows.Data;

namespace MeteoApp.WPF.Converters
{
    // Convertit l'horodatage Unix (long) en un nom de jour lisible.
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long unixTime)
            {
                // 1. Conversion de l'horodatage Unix en DateTimeOffset
                var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);

                // 2. Conversion en heure locale de l'utilisateur
                var localDateTime = dateTimeOffset.LocalDateTime;

                // 3. Formater le résultat (ex: "Mercredi", ou le temps si c'est pour l'heure)
                // Ici, on affiche le nom du jour
                return localDateTime.ToString("ddd d MMMM");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
