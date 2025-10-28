using MeteoApp.Core.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels.Base;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace MeteoApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Dépendance injectée : le contrat de service API
        private readonly IMeteoServices _meteoService;

        // --- Propriétés liées au XAML (la View) ---

        private string _nomVille = "";
        // Utilisation du SetProperty pour notifier la View
        public string NomVille
        {
            get => _nomVille;
            set => SetProperty(ref _nomVille, value);
        }

        private MeteoJour _meteoDuJour;
        public MeteoJour MeteoDuJour
        {
            get => _meteoDuJour;
            set => SetProperty(ref _meteoDuJour, value);
        }

        private bool _isBusy;
        // Propriété pour gérer l'état de chargement de l'UI
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        // Vérification si des données on été trouvés/recherchées
        private bool _hasMeteoData;
        public bool HasMeteoData
        {
            get => _hasMeteoData;
            set
            {
                // 1. Utilisez SetProperty pour mettre à jour la valeur et notifier le changement de HasMeteoData
                if (SetProperty(ref _hasMeteoData, value))
                {
                    OnPropertyChanged(nameof(HasNoMeteoData));
                }
            }
        }

        public bool HasNoMeteoData => !HasMeteoData;

        // --- Commande liée au XAML (le Bouton) ---

        // La propriété que la View va binder (Command="{Binding RechercherMeteoCommand}")
        public ICommand RechercherMeteoCommand { get; }

        // Constructeur qui reçoit le service via l'Injection de Dépendances (DI)
        public MainViewModel(IMeteoServices meteoService)
        {
            _meteoService = meteoService;

            // Initialisation de la Commande avec la méthode d'exécution asynchrone
            // SimpleCommand attend une Action (synchrone), on l'encapsule dans Task.Run
            RechercherMeteoCommand = new SimpleCommand(() => Task.Run(async () => await ExecuteRechercherMeteoAsync()));
        }

        // Logique d'exécution de la recherche, asynchrone pour ne pas bloquer l'UI
        private async Task ExecuteRechercherMeteoAsync()
        {
            if (IsBusy) return;

            IsBusy = true; // Début du chargement (mettra à jour l'UI)
            MeteoDuJour = null;

            try
            {
                // Appel au service API
                var result = await _meteoService.GetMeteoAsync(NomVille);

                if (result == null)
                {
                    Debug.WriteLine($"[Erreur Recherche] Météo pour '{NomVille}' introuvable.");
                }

                MeteoDuJour = result; // Mise à jour des données (notifie la View)
                HasMeteoData = (result != null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Erreur Système] Une erreur est survenue : {ex.Message}");
            }
            finally
            {
                IsBusy = false; // Fin du chargement (mettra à jour l'UI)
            }
        }
    }
}