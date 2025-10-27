using MeteoApp.Core.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MeteoApp.Services
{
    public class OpenMeteoService : IMeteoServices
    {
        private readonly string _apiKey; 
        private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";

        // HttpClient doit être réutilisé pour une bonne gestion des ressources.
        private readonly HttpClient _httpClient = new HttpClient();

        // Le service demande IConfiguration par DI
        public OpenMeteoService(IConfiguration configuration)
        {
            // Lecture de la clé à partir de la section 'OpenWeatherMap:ApiKey' dans secrets.json
            _apiKey = configuration["OpenWeatherMap:ApiKey"]
                      ?? throw new InvalidOperationException("Clé API OWM introuvable dans la configuration.");
        }

        public async Task<MeteoJour> GetMeteoAsync(string nomVille)
        {
            if (string.IsNullOrWhiteSpace(nomVille))
            {
                return null;
            }

            // Construction de l'URL avec les paramètres : Ville, Clé, et Unités (metric = Celsius)
            string url = $"{BASE_URL}?q={nomVille}&appid={_apiKey}&units=metric&lang=fr";

            try
            {
                // Bonne pratique : utilisation de GetFromJsonAsync pour la requête et la désérialisation
                // C'est la méthode asynchrone qui empêche l'UI de bloquer.
                var meteoData = await _httpClient.GetFromJsonAsync<MeteoJour>(url);

                return meteoData;
            }
            catch (HttpRequestException ex)
            {
                // Gestion des erreurs HTTP (ex: 404 Not Found, 500 Internal Server Error)
                System.Diagnostics.Debug.WriteLine($"Erreur API pour {nomVille} : {ex.Message}");
                return null;
            }
            // On pourrait ajouter d'autres catch pour la désérialisation ou autres.
        }
    }
}
