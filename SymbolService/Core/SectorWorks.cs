using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SymbolService.Logs;
using SymbolService.Models;
using SymbolService.Models.Context;

namespace SymbolService.Core
{
    public class SectorWorks
    {
        public static int LoadSectors()
        {
            Log.WriteLog(new LogEvent("Sector - LoadSectors:", "Start"));
            var max = GetSectorMax();

            var db = new SymbolContext();

            var today = DateTime.Now.ToShortDateString();
            var yesterday = DateTime.Now.AddDays(-1);
            if (db.Sectors.Any(p => p.Date > yesterday))
            {
                Log.WriteLog(new LogEvent("SectorWorks - LoadSectors:", "End - Already ran for this date"));
                return -1;
            }
            //if (db.Sectors.Any()) return;

            var sUri = URLs.GetSectorCsv;

            // this creates a string from a stream reader
            var sTopLinks = WebWorks.GetResponse(sUri);

            //...so we need to re-stream it
            byte[] byteArray = Encoding.ASCII.GetBytes(sTopLinks);
            var stream = new MemoryStream(byteArray);

            StreamReader reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var myrow = reader.ReadLine();

                if (reader.EndOfStream) break;

                var cols = myrow.Split(',');

                // skip header
                if (cols[0].IndexOf("Sector", System.StringComparison.Ordinal) > -1) continue;

                var sector = new Sector();
                sector.Date = DateTime.Now.Date;
                sector.Name = cols[0].Replace("\"", "");
                sector.OneDayPriceChgPerCent = Convert.ToDecimal(cols[1]);
                sector.MarketCap = cols[2];
                sector.PriceToEarnings = Convert.ToDecimal(cols[3]);
                sector.ROEPerCent = Convert.ToDecimal(cols[4]);
                sector.DivYieldPerCent = Convert.ToDecimal(cols[5] == "\"NA\"" ? "-1" : cols[5]);
                sector.DebtToEquity = Convert.ToDecimal(cols[6] == "\"NA\"" ? "-1" : cols[6]);
                sector.PriceToBook = Convert.ToDecimal(cols[7]);
                sector.NetProfitMarginMrq = Convert.ToDecimal(cols[8]);
                sector.PriceToFreeCashFlowMrq = Convert.ToDecimal(cols[9]);

                db.Sectors.Add(sector);
            }

            db.SaveChanges();
            db.Dispose();
            reader.Close();

            Log.WriteLog(new LogEvent("SectorWorks - LoadSectors:", "End"));
            return max;
        }

        private static int GetSectorMax()
        {
            int selected = 0;

            using (var db = new SymbolContext())
            {
                try
                {
                    selected = (from t in db.Sectors
                                select t.Id).Max();
                }
                catch (Exception ex)
                {
                    Log.WriteLog(new LogEvent("SectortWorks - GetSectortMax Error:", ex.ToString()));
                }

                return selected;
            }
        }

        public static Dictionary<string, string> GetSectorNames()
        {
            SymbolContext db = new SymbolContext();
            Dictionary<string, string> sectorNames = new Dictionary<string, string>();
            DateTime maxDate = db.Sectors.Max(d => d.Date);

            Log.WriteLog(new LogEvent(string.Format("SectorWorks - GetSectorNames() for date {0}", maxDate), ""));

            try
            {
                var sectors = db.Sectors.Where(s => s.Date == maxDate).ToList();
                for (int i = 0; i < sectors.Count(); i++)
                {
                    sectorNames.Add(sectors[i].Id.ToString(CultureInfo.InvariantCulture), sectors[i].Name);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(new LogEvent("SectortWorks - GetSectorNames Error:", ex.ToString()));
            }

            return sectorNames;
        }
    }
}