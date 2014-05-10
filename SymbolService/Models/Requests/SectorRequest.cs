using System.Collections.Generic;
using Newtonsoft.Json;
using SymbolService.Models.WCFModels;

namespace SymbolService.Models.Requests
{
    public class SectorRequest : BaseRequestData
    {
        [JsonProperty(PropertyName = "sectors")]
        public List<WCFSector> sectors { get; set; }
    }
}
