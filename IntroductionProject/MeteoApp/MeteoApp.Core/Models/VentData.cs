using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MeteoApp.Core.Models
{
    public class VentData
    {
        [JsonPropertyName("speed")]
        public double Vitesse { get; set; }
    }
}
