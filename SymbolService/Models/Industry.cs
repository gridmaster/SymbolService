using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SymbolService.Models
{
    public class Industry
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "industryId")]
        public int IndustryId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}