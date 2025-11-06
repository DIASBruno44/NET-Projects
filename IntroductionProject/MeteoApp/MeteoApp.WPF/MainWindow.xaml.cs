using System.Windows;
using MeteoApp.ViewModels;

namespace MeteoApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            // 💡 Définition du DataContext : le XAML peut maintenant "voir" le ViewModel
            DataContext = viewModel;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DataContext is IDisposable disposableViewModel)
            {
                // Appel de la méthode Dispose() du MainViewModel
                disposableViewModel.Dispose();
            }
        }
    }
}