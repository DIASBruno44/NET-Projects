using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace MeteoApp.Core.Models
{
    // Ce modèle est le conteneur principal de la réponse API pour les prévisions
    public class Previsions
    {
        // La propriété "list" contient un tableau de prévisions.
        // Chaque élément de cette liste a la même structure que votre modèle MeteoJour existant.
        [JsonPropertyName("list")]
        public List<MeteoJour> ListePrevisions { get; set; }

        // La propriété "city"
        [JsonPropertyName("city")]
        public Ville VilleInfo { get; set; }
    }
}
