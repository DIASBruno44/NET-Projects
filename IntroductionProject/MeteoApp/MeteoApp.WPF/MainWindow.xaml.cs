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
        }
    }
}