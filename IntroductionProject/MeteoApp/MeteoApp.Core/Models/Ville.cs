using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models
{
    public class Ville
    {
        [JsonPropertyName("name")]
        public string NomVille { get; set; }

    }
}
