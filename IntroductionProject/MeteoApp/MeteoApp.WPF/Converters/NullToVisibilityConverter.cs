using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MeteoApp.WPF.Converters
{
    // Convertit un objet non-nul en Visibility.Visible et un objet null en Visibility.Collapsed.
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si la valeur est null (pas de données sélectionnées), on masque.
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            // Si la valeur n'est pas null (il y a des données), on affiche.
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Conversion non requise pour la visibilité
            return DependencyProperty.UnsetValue;
        }
    }
}
