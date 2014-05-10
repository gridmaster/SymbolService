using System.Collections.Generic;

namespace TestSymbolService.Models.Requests
{
    public class SectorRequest : BaseRequestData
    {
        public List<Sector> sector { get; set; }
    }
}
