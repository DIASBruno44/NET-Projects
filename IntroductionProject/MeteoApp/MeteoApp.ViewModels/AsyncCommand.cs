using System;
using System.Windows.Input;

namespace MeteoApp.ViewModels
{
    // Implémentation basique de ICommand
    public class AsyncCommand : ICommand
    {
        // L'Action (méthode) à exécuter
        private readonly Func<Task> _execute;

        // Cet événement est requis par WPF, il signale que l'état du bouton pourrait changer
        public event EventHandler CanExecuteChanged;

        public AsyncCommand(Func<Task> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        // Simplifié, on suppose que l'action est toujours possible
        public bool CanExecute(object parameter) => true;

        // Exécute la méthode du ViewModel
        public async void Execute(object parameter) => await _execute();
    }
}