using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using MeteoApp.Services;
using MeteoApp.ViewModels;

namespace MeteoApp.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            // Configure l'hôte et tous les services (DI)
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 1. Enregistrement des Services (Contrat vers Implémentation)
                    // Quand on demande IMeteoService, le conteneur fournit OpenMeteoService.
                    services.AddSingleton<IMeteoServices, OpenMeteoService>();

                    // 2. Enregistrement des ViewModels
                    services.AddTransient<MainViewModel>();

                    // 3. Enregistrement de la View (Fenêtre)
                    services.AddSingleton<MainWindow>();

                })
                .Build();
        }

        // Démarre l'hôte et affiche la fenêtre principale récupérée par DI
        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Récupère la MainWindow : Le DI injecte automatiquement le MainViewModel dans son constructeur.
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }
            base.OnExit(e);
        }
    }

}
