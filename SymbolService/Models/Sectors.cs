using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SymbolService.Models
{
    public class Sectors
    {
        [JsonProperty(PropertyName = "sector")]
        public List<Sector> Sectorz { get; set; }
    }
}