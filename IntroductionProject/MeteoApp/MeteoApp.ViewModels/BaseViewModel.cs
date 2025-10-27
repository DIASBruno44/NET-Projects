using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace MeteoApp.ViewModels.Base
{
    // C'est la classe gérant les notifications
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Cet événement est écouté par la View (le XAML)
        public event PropertyChangedEventHandler PropertyChanged;

        // Méthode appelée pour notifier l'UI d'un changement
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Méthode helper pour définir la valeur ET notifier si elle a changé
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false; // Pas de changement

            storage = value;
            OnPropertyChanged(propertyName); // C'est ici que la notification est envoyée
            return true;
        }
    }
}
