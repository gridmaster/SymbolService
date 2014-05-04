using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TestSymbolService.Models
{
    public class Sector
    {
        [Key]
        public int id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "industry")]
        public List<Industry> Industries { get; set; } 
    }
}