using System;
using System.Windows.Input;

namespace MeteoApp.ViewModels
{
    // Implémentation basique de ICommand
    public class SimpleCommand : ICommand
    {
        // L'Action (méthode) à exécuter
        private readonly Action _execute;

        // Cet événement est requis par WPF, il signale que l'état du bouton pourrait changer
        public event EventHandler CanExecuteChanged;

        public SimpleCommand(Action execute) => _execute = execute;

        // Simplifié, on suppose que l'action est toujours possible
        public bool CanExecute(object parameter) => true;

        // Exécute la méthode du ViewModel
        public void Execute(object parameter) => _execute();
    }
}