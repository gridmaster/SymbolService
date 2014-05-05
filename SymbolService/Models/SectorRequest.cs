using System.Collections.Generic;

namespace SymbolService.Models
{
    public class SectorRequest : BaseRequestData
    {
        public List<Sector> sector { get; set; }
    }
}
