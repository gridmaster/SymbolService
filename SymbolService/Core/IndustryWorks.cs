using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SymbolService.Logs;
using SymbolService.Models;
using SymbolService.Models.Context;

namespace SymbolService.Core
{
    public class IndustryWorks
    {
        public static int LoadIndustriesBySector()
        {
            Log.WriteLog(new LogEvent("IndustryWorks - LoadIndustriesBySector:", "Start"));
            var db = new SymbolContext();

            var max = GetIndustryMax();

            var today = DateTime.Now.ToString("MM/dd/yyyy");
            var yesterday = DateTime.Today.AddDays(-1);
            if (db.Industries.Any(p => p.Date > yesterday))
            {
                Log.WriteLog(new LogEvent("IndustryWorks - LoadIndustriesBySector:", "End - Already ran for this date"));
                return -1;
            }

            var sUri = URLs.GetIndustryBySectorCsv;

            var sid = db.LastUpdates.Where(s => s.Name == "Sectors").OrderByDescending(s => s.Id).Select(s => s.Count).First();

            var Sectors = db.Sectors.Where(s => s.Id > sid).OrderBy(s => s.Id);

            int i = 1;
            foreach (Sector sector in Sectors)
            {
                // this creates a string from a stream reader
                var sTopLinks = WebWorks.GetResponse(sUri.Replace("!^$^!", i.ToString()));
                i++;

                //...so we need to re-stream it
                byte[] byteArray = Encoding.ASCII.GetBytes(sTopLinks);
                var stream = new MemoryStream(byteArray);

                var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var myrow = reader.ReadLine();

                    if (reader.EndOfStream) break;

                    myrow = StringWorks.fixComma(myrow);

                    var cols = myrow.Split(',');

                    var wtf = cols[0].IndexOf("Ind", System.StringComparison.Ordinal);

                    // skip header
                    if (cols[0].IndexOf("Ind", System.StringComparison.Ordinal) > -1) continue;

                    var industry = new Industry();
                    industry.SectorId = sector.Id;
                    industry.Date = DateTime.Now.Date;
                    industry.Name = cols[0].Replace("!^$^!", ",").Replace("\"", ""); ;
                    industry.OneDayPriceChgPerCent = System.Convert.ToDecimal(cols[1]);
                    industry.MarketCap = cols[2];
                    industry.PriceToEarnings = System.Convert.ToDecimal(cols[3]);
                    industry.ROEPerCent = System.Convert.ToDecimal(cols[4]);
                    industry.DivYieldPerCent = System.Convert.ToDecimal(cols[5] == "\"NA\"" ? "-1" : cols[5]);
                    industry.DebtToEquity = System.Convert.ToDecimal(cols[6] == "\"NA\"" ? "-1" : cols[6]);
                    industry.PriceToBook = System.Convert.ToDecimal(cols[7]);
                    industry.NetProfitMarginMrq = System.Convert.ToDecimal(cols[8]);
                    industry.PriceToFreeCashFlowMrq = System.Convert.ToDecimal(cols[9]);

                    db.Industries.Add(industry);
                }


                reader.Close();
            }

            db.SaveChanges();
            Log.WriteLog(new LogEvent("IndustryWorks - LoadIndustriesBySector:", "End"));
            return max;
        }

        public static int LoadIndustries()
        {
            Log.WriteLog(new LogEvent("IndustryWorks - LoadIndustries:", "Start"));
            var db = new SymbolContext();

            var max = GetIndustryMax();

            if (db.Industries.Any(p => p.Date > DateTime.Today.AddDays(-1)))
            {
                Log.WriteLog(new LogEvent("IndustryWorks - LoadIndustries:", "End - Already ran for this date"));
                return max;
            }

            var sUri = URLs.GetIndustryCsv;

            // this creates a string from a stream reader
            var sTopLinks = WebWorks.GetResponse(sUri);

            //...so we need to re-stream it
            byte[] byteArray = Encoding.ASCII.GetBytes(sTopLinks);
            var stream = new MemoryStream(byteArray);

            var reader = new StreamReader(stream);
            //sTopLinks = reader.ReadToEnd();

            while (!reader.EndOfStream)
            {
                var myrow = reader.ReadLine();

                if (reader.EndOfStream) break;

                myrow = StringWorks.fixComma(myrow);

                var cols = myrow.Split(',');

                var wtf = cols[0].IndexOf("Ind", System.StringComparison.Ordinal);

                // skip header
                if (cols[0].IndexOf("Ind") > -1) continue;

                var industry = new Industry();
                industry.Date = DateTime.Now.Date;
                industry.Name = cols[0].Replace("!^$^!", ",").Replace("\"", ""); ;
                industry.OneDayPriceChgPerCent = System.Convert.ToDecimal(cols[1]);
                industry.MarketCap = cols[2];
                industry.PriceToEarnings = System.Convert.ToDecimal(cols[3]);
                industry.ROEPerCent = System.Convert.ToDecimal(cols[4]);
                industry.DivYieldPerCent = System.Convert.ToDecimal(cols[5] == "\"NA\"" ? "-1" : cols[5]);
                industry.DebtToEquity = System.Convert.ToDecimal(cols[6] == "\"NA\"" ? "-1" : cols[6]);
                industry.PriceToBook = System.Convert.ToDecimal(cols[7]);
                industry.NetProfitMarginMrq = System.Convert.ToDecimal(cols[8]);
                industry.PriceToFreeCashFlowMrq = System.Convert.ToDecimal(cols[9]);

                db.Industries.Add(industry);

            }

            db.SaveChanges();

            Log.WriteLog(new LogEvent("IndustryWorks - LoadIndustries:", "End"));
            return max;
        }

        private static int GetIndustryMax()
        {
            int selected = 0;

            using (var db = new SymbolContext())
            {
                try
                {
                    selected = (from t in db.Industries
                                select t.Id).Max();
                }
                catch (Exception ex)
                {
                    Log.WriteLog(new LogEvent("IndustryWorks - GetIndustryMax Error:", ex.ToString()));
                }

                return selected;
            }
        }
    }
}