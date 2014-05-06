using System.Collections.Generic;
using Newtonsoft.Json;

namespace SymbolService.Models
{
    public class SectorRequest : BaseRequestData
    {
        [JsonProperty(PropertyName = "sectors")]
        public List<Sector> sectors { get; set; }
    }
}
