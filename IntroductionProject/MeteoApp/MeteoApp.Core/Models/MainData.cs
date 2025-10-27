using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MeteoApp.Core.Models
{
    public class MainData
    {
        [JsonPropertyName("temp")]
        public double TemperatureActuelle { get; set; }

        [JsonPropertyName("feels_like")]
        public double TemperatureRessentie { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidite { get; set; }
    }
}
