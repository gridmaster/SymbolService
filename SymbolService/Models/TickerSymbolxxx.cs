using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SymbolService.Logs;
using SymbolService.Models.Context;

namespace SymbolService.Models
{
    public class TickerSymbolxxx
    {
        public int Id { get; set; }

        [Required]
        public string Symbol { get; set; }

        public string Name { get; set; }
        public string ExchangeId { get; set; }
        public string ExchangeName { get; set; }
        public string Industry { get; set; }
        public string Sector { get; set; }
        public string KeyStat { get; set; }

        public DateTime Date { get; set; }

        public DateTime? Deleted { get; set; }

        [Timestamp]
        public Byte[] TimeStamp { get; set; }




        #region Not implemented
        //public List<TickerSymbol> GetOptionTickerSymbols()
        //{
        //    var db = new SymbolContext();
        //    var selected = (from t in db.TickerSymbols
        //                    where new[] { "NasdaqGS", "NasdaqGM", "NYSEArca", "NasdaqCM",
        //                        "NYSE", "NYSE Amex" }.Contains(t.ExchangeName)
        //                    select t).ToList();

        //    return selected;
        //}

        ////[GetShortKeyStatSymbolList]
        //public List<SymbolAndId> GetShortKeyStatSymbolList()
        //{
        //    var db = new SymbolContext();

        //    var selected = new List<SymbolAndId>();
        //    var conn = new SqlConnection(Market.Strategy.Business.SiteWorks.GetConnectionString("SymbolContext"));
        //    conn.Open();
        //    SqlCommand cmd = new SqlCommand("GetShortKeyStatSymbolList", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    var response = cmd.ExecuteReader();

        //    while (response.Read())
        //    {
        //        try
        //        {
        //            var ts = new SymbolAndId();
        //            ts.Id = (int)response["Id"];
        //            ts.Symbol = response["Symbol"].ToString();
        //            selected.Add(ts);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.WriteLog(new LogEvent("TickerSymbol - GetKeyStatsTickerSymbols Error:", ex.ToString()));
        //        }
        //    }
        //    return selected;
        //}

        //public List<TickerSymbol> GetKeyStatSymbols()
        //{
        //    var conn = new SqlConnection(Market.Strategy.Business.SiteWorks.GetConnectionString("SymbolContext"));
        //    conn.Open();
        //    SqlCommand cmd = new SqlCommand("GetKeyStatSymbols", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    var response = cmd.ExecuteReader();
        //    var ts = new List<TickerSymbol>();

        //    while (response.Read())
        //    {
        //        var what = response[0].ToString();
        //    }

        //    return ts;
        //}

        //public List<TickerSymbol> GetKeyStatsTickerSymbols()
        //{
        //    var db = new SymbolContext();

        //    var selected = new List<TickerSymbol>();
        //    while (selected.Count == 0)
        //    {
        //        try
        //        {
        //            selected = (from t in db.TickerSymbols
        //                        where t.KeyStat == "true"
        //                        select t).ToList();
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.WriteLog(new LogEvent("TickerSymbol - GetKeyStatsTickerSymbols Error:", ex.ToString()));
        //        }
        //    }
        //    return selected;
        //}

 
        //public void GetDailyStatsTickerSymbols(ref List<TickerSymbol> selected)
        //{
        //    var db = new SymbolContext();

        //    var symbols = (from d in db.DailyStats
        //                   // orderby t.Symbol
        //                   select d.Symbol).Distinct()
        //                   .OrderBy(s => s).ToArray();

        //    selected = (from t in db.TickerSymbols
        //                where symbols.Contains(t.Symbol)
        //                select t).ToList();

        //    return;
        //}

        //public List<TickerSymbol> GetDailyStatsTickerSymbols()
        //{
        //    var db = new SymbolContext();

        //    var symbols = (from d in db.DailyStats
        //                   // orderby t.Symbol
        //                   select d.Symbol).Distinct().OrderBy(s => s).ToArray();

        //    var selected = (from t in db.TickerSymbols
        //                    where symbols.Contains(t.Symbol)
        //                    select t).ToList();

        //    return selected;
        //}
        #endregion Not implemented
    }
}