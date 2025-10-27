using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MeteoApp.Core.Models
{
    public class DescriptionMeteo
    {
        [JsonPropertyName("main")]
        public string Titre { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } // La View s'y lie (via Descriptions[0])

        [JsonPropertyName("icon")]
        public string IconeCode { get; set; }
    }
}
