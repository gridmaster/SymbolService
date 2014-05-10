using System.Collections.Generic;
using Newtonsoft.Json;
using SymbolService.Models.WCFModels;

namespace SymbolService.Models.Requests
{
    public class IndustryRequest : BaseRequestData
    {
        [JsonProperty(PropertyName = "industries")]
        public List<WCFIndustry> industries { get; set; }
    }
}