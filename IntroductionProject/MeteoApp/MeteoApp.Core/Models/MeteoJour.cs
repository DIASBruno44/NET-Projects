using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MeteoApp.Core.Models
{
    public class MeteoJour
    {
        [JsonPropertyName("name")]
        public string NomVille { get; set; } // La View s'y lie

        [JsonPropertyName("main")]
        public MainData DonnéesPrincipales { get; set; } // La View s'y lie (pour la température)

        [JsonPropertyName("weather")]
        public List<DescriptionMeteo> Descriptions { get; set; } // La View s'y lie (pour la description)

        [JsonPropertyName("wind")]
        public VentData Vent { get; set; }
    }
}
