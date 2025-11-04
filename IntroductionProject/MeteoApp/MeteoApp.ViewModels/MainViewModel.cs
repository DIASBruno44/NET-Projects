using MeteoApp.Core.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels.Base;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Windows;

namespace MeteoApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Dépendance injectée : le contrat de service API
        private readonly IMeteoServices _meteoService;
        private const int RefreshIntervalMinutes = 10;

        //Un drapeau pour contrôler l'arrêt de la boucle
        private CancellationTokenSource _cancellationTokenSource;
        public bool FirstAPI_Connceciton = false;

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

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

            RechercherMeteoCommand = new AsyncCommand(ExecuteRechercherMeteoAsync); 
            StartAutoRefresh();

        }
        private void StartAutoRefresh()
        {
            // Annule la boucle précédente si elle existait
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            // On lance la boucle sans bloquer le constructeur
            Task.Run(() => AutoRefreshLoop(_cancellationTokenSource.Token));
        }

        private async Task AutoRefreshLoop(CancellationToken cancellationToken)
        {
            // Lance la première recherche immédiatement
            await ExecuteRechercherMeteoAsync();

            // Boucle infinie, vérifiée par le token d'annulation
            while (!cancellationToken.IsCancellationRequested)
            {
                // 1. Pause asynchrone non bloquante (la magie du Task.Delay)
                await Task.Delay(TimeSpan.FromMinutes(RefreshIntervalMinutes), cancellationToken);

                // Vérifie la demande d'annulation après la pause
                if (cancellationToken.IsCancellationRequested) break;

                // 2. Exécute la recherche si l'application n'est pas occupée
                if (!IsBusy && !string.IsNullOrWhiteSpace(NomVille))
                {
                    await ExecuteRechercherMeteoAsync();
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }


        // Logique d'exécution de la recherche, asynchrone pour ne pas bloquer l'UI
        private async Task ExecuteRechercherMeteoAsync()
        {
            if (IsBusy) return;

            if (!FirstAPI_Connceciton)
            {
                
                ErrorMessage = "";
                // On pourrait aussi notifier le XAML pour un rafraîchissement
                OnPropertyChanged(nameof(ErrorMessage));
                FirstAPI_Connceciton = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(NomVille))
            {
                ErrorMessage = "Veuillez entrer le nom d'une ville.";
                // On pourrait aussi notifier le XAML pour un rafraîchissement
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            IsBusy = true;
            MeteoDuJour = null;
            ErrorMessage = null; // ⬅️ Réinitialise l'erreur avant la recherche


            try
            {
                var resultJour = await _meteoService.GetMeteoAsync(NomVille);

                if (resultJour == null)
                {
                    // 💡 Affichage de l'erreur à l'utilisateur via la propriété
                    ErrorMessage = $"Désolé, la ville '{NomVille}' est introuvable ou il y a eu une erreur de connexion.";
                    HasMeteoData = false;
                }
                else
                {
                    MeteoDuJour = resultJour;
                    HasMeteoData = true;
                }

                // ... (votre code pour les prévisions irait ici)
            }
            catch (Exception ex)
            {
                // En cas d'exception système (ex: JSON mal formé)
                ErrorMessage = "Une erreur système inattendue est survenue.";
                System.Diagnostics.Debug.WriteLine($"[Erreur Système] : {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
            


        }
    }
}