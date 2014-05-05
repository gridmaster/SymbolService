using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TestSymbolService.Models
{
    public class Industry
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }

        [JsonProperty(PropertyName = "industryId")]
        public int industryId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
    }
}