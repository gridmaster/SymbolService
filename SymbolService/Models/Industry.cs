using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SymbolService.Models
{
    public class Industry
    {
        [Key]
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "IndustryId")]
        public int IndustryId { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
    }
}