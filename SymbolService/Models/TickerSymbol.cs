using System;

namespace SymbolService.Models
{
    public class TickerSymbol
    {
        public int Id { get; set; }
	    public string Symbol { get; set; }
	    public string Name { get; set; }
	    public string ExchangeId { get; set; }
	    public string ExchangeName { get; set; }
	    public string Industry { get; set; }
	    public string Sector { get; set; }
	    public DateTime Date { get; set; }
        public DateTime TimeStamp { get; set; }
	    public bool ValidData { get; set; }
	    public string KeyStat { get; set; }
        //public bool History { get; set; }
        //public DateTime LastHistoryDate { get; set; }
    }
}
