using System.Collections.Generic;

namespace TestSymbolService.Models
{
    public class SectorRequest : BaseRequestData
    {
        public List<Sector> sector { get; set; }
    }
}
