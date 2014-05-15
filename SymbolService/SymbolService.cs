using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using SymbolService.BulkLoad;
using SymbolService.Core;
using SymbolService.Logs;
using SymbolService.Models;
using SymbolService.Models.Context;
using SymbolService.Models.Requests;
using SymbolService.Models.ViewModels;

namespace SymbolService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SymbolService : ISymbolService
    {
        private SymbolContext db = new SymbolContext();

        [WebGet(UriTemplate = "/Sectors", ResponseFormat = WebMessageFormat.Json)]
        public Sectors GetSectors()
        {
            DateTime maxDate = db.Sectors.Max(d => d.Date);

            Log.WriteLog(new LogEvent(string.Format("SymbolService - GetSectors() for date {0}", maxDate), " - do bulk insert"));

            string json = string.Empty;

            try
            {
                var sectors = db.Sectors.Where(s => s.Date == maxDate);

                Sectors sectorList = new Sectors();

                foreach (var sector in sectors)
                {
                    Sector t = new Sector
                        {
                            Id = sector.Id,
                            Date = sector.Date,
                            Name = sector.Name,
                            OneDayPriceChgPerCent = sector.OneDayPriceChgPerCent,
                            MarketCap = sector.MarketCap,
                            PriceToEarnings = sector.PriceToEarnings,
                            ROEPerCent = sector.ROEPerCent,
                            DivYieldPerCent = sector.DivYieldPerCent,
                            DebtToEquity = sector.DebtToEquity,
                            PriceToBook = sector.PriceToBook,
                            NetProfitMarginMrq = sector.NetProfitMarginMrq,
                            PriceToFreeCashFlowMrq = sector.PriceToFreeCashFlowMrq
                        };
                    sectorList.Add(t);
                }
                json = JsonConvert.SerializeObject(sectorList).Replace("T00:00:00", "");
            }
            catch (Exception ex)
            {
                json = string.Format("Get Sectors threw error: {0}", ex.Message);
                Log.WriteLog(new LogEvent(string.Format("SymbolService - Get Sectors for date {0}", maxDate), json));
            }

            return JsonConvert.DeserializeObject<Sectors>(json);
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/LoadSectors")]
        public string LoadSectors(SectorRequest sectors)
        {
            Log.WriteLog(new LogEvent(
                string.Format("SymbolService - LoadSectors(): range start: {0} - count: {1}", sectors.sectors[0].Id, sectors.sectors.Count), " - do bulk insert"));
            
            string result = string.Empty;

            if (sectors.token != "bc2afdc0-6f68-497a-9f6c-4e261331c256")
            {
                result = "You didn't say the magic word!";
            }
            else
            {
                result = "Got 'em!";
                Sectors bulkSectors = new Sectors();

                for (int i = 0; i < sectors.sectors.Count; i++)
                {
                    Sector sector = sectors.sectors[i].ConvertToSector();
                    bulkSectors.Add(sector);
                }

                BulkLoadSector bls = new BulkLoadSector();

                var dt = bls.ConfigureDataTable();

                dt = bls.LoadDataTableWithSectors(bulkSectors, dt);

                bls.BulkCopy<Sectors>(dt);
            }

            return result;
        }

        [WebInvoke(Method = "POST", UriTemplate = "/LoadDailySectors", ResponseFormat = WebMessageFormat.Json)]
        public string LoadDailySectors(BasicRequest basicRequest)
        {
            Log.WriteLog(new LogEvent("SymbolService - LoadDailySectors", "insert"));

            string result = string.Empty;

            if (basicRequest.token != "bc2afdc0-6f68-497a-9f6c-4e261331c256")
            {
                result = "You didn't say the magic word!";
            }
            else
            {
                var maxCount = SectorWorks.LoadSectors();
                var today = DateTime.Now.ToShortDateString();

                if (maxCount > 0)
                {
                    var myDate = new LastUpdate();
                    myDate.Name = "Sectors";
                    myDate.Date = today;
                    myDate.Count = maxCount;
                    db.LastUpdates.Add(myDate);
                    db.SaveChanges();
                }
            }
            return result;
        }

        [WebGet(UriTemplate = "/Industries", ResponseFormat = WebMessageFormat.Json)]
        public Industries GetIndustries()
        {
            DateTime maxDate = db.Industries.Max(d => d.Date);

            Log.WriteLog(new LogEvent(string.Format("SymbolService - GetIndustries() for date {0}", maxDate), " - do bulk insert"));

            string json = string.Empty;

            try
            {
                var industries = db.Industries.Where(i => i.Date == maxDate);

                Industries industryList = new Industries();

                foreach (var industry in industries)
                {
                    Industry t = new Industry
                        {
                            Id = industry.Id,
                            SectorId = industry.SectorId,
                            Date = industry.Date,
                            Name = industry.Name,
                            OneDayPriceChgPerCent = industry.OneDayPriceChgPerCent,
                            MarketCap = industry.MarketCap,
                            PriceToEarnings = industry.PriceToEarnings,
                            ROEPerCent = industry.ROEPerCent,
                            DivYieldPerCent = industry.DivYieldPerCent,
                            DebtToEquity = industry.DebtToEquity,
                            PriceToBook = industry.PriceToBook,
                            NetProfitMarginMrq = industry.NetProfitMarginMrq,
                            PriceToFreeCashFlowMrq = industry.PriceToFreeCashFlowMrq
                        };
                    industryList.Add(t);
                }

                json = JsonConvert.SerializeObject(industryList).Replace("T00:00:00", "");
            }
            catch (Exception ex)
            {
                json = string.Format("Get Industries threw error: {0}", ex.Message);
                Log.WriteLog(new LogEvent(string.Format("SymbolService - Get Industries for date {0}", maxDate), json));
            }
            return JsonConvert.DeserializeObject<Industries>(json);
        }

        [WebGet(UriTemplate = "/IndustriesWithSectorName", ResponseFormat = WebMessageFormat.Json)]
        public IndustryView IndustryWithSectorName()
        {
            DateTime maxDate = db.Industries.Max(d => d.Date);
            Dictionary<string, string> sectorNames = SectorWorks.GetSectorNames();

            Log.WriteLog(new LogEvent(string.Format("SymbolService - GetIndustries() for date {0}", maxDate), " - do bulk insert"));

            string json = string.Empty;

            try
            {
                var industries = db.Industries.Where(i => i.Date == maxDate);

                List<IndustryView> industryList = new List<IndustryView>();

                foreach (var industry in industries)
                {
                    string sectorName = "Unknown";
                    if (sectorNames.ContainsKey(industry.SectorId.ToString(CultureInfo.InvariantCulture)))
                        sectorName = sectorNames[industry.SectorId.ToString(CultureInfo.InvariantCulture)];

                    IndustryView t = new IndustryView
                    {
                        Id = industry.Id,
                        Sector = sectorName,
                        Date = industry.Date,
                        Name = industry.Name,
                        OneDayPriceChgPerCent = industry.OneDayPriceChgPerCent,
                        MarketCap = industry.MarketCap,
                        PriceToEarnings = industry.PriceToEarnings,
                        ROEPerCent = industry.ROEPerCent,
                        DivYieldPerCent = industry.DivYieldPerCent,
                        DebtToEquity = industry.DebtToEquity,
                        PriceToBook = industry.PriceToBook,
                        NetProfitMarginMrq = industry.NetProfitMarginMrq,
                        PriceToFreeCashFlowMrq = industry.PriceToFreeCashFlowMrq
                    };
                    industryList.Add(t);
                }
                json = JsonConvert.SerializeObject(industryList).Replace("T00:00:00", "");
            }
            catch (Exception ex)
            {
                json = string.Format("Get Industries threw error: {0}", ex.Message);
                Log.WriteLog(new LogEvent(string.Format("SymbolService - Get Industries for date {0}", maxDate), json));
            }
            return JsonConvert.DeserializeObject<IndustryView>(json);
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/LoadIndustries")]
        public string LoadIndustries(IndustryRequest industries)
        {
            Log.WriteLog(new LogEvent(
                string.Format("SymbolService - LoadIndustries(): range start: {0} - count: {1}", industries.industries[0].Id, industries.industries.Count), " - do bulk insert"));

            string result = string.Empty;

            if (industries.token != "bc2afdc0-6f68-497a-9f6c-4e261331c256")
            {
                result = "You didn't say the magic word!";
            }
            else
            {
                result = "Got 'em!"; 
                Industries bulkIndustries = new Industries();

                for (int i = 0; i < industries.industries.Count; i++)
                {
                    Industry industry = industries.industries[i].ConvertToIndustry();
                    bulkIndustries.Add(industry);
                }

                BulkLoadIndustry bli = new BulkLoadIndustry();

                var dt = bli.ConfigureDataTable();

                dt = bli.LoadDataTableWithIndustries(bulkIndustries, dt);

                bli.BulkCopy<Industries>(dt);
            }

            return result;
        }

        [WebInvoke(Method = "POST", UriTemplate = "/LoadDailyIndustries", ResponseFormat = WebMessageFormat.Json)]
        public string LoadDailyIndustries(BasicRequest basicRequest)
        {
            Log.WriteLog(new LogEvent("SymbolService - LoadDailyIndustries", "insert"));

            string result = string.Empty;

            if (basicRequest.token != "bc2afdc0-6f68-497a-9f6c-4e261331c256")
            {
                result = "You didn't say the magic word!";
            }
            else
            {
                var maxCount = IndustryWorks.LoadIndustriesBySector();
                var today = DateTime.Now.ToShortDateString();

                if (maxCount > 0)
                {
                    var myDate = new LastUpdate();
                    myDate.Name = "Industries";
                    myDate.Date = today;
                    myDate.Count = maxCount;
                    db.LastUpdates.Add(myDate);
                    db.SaveChanges();
                }
            }
            return result;
        }

        [WebGet(UriTemplate = "/TickerSymbols", ResponseFormat = WebMessageFormat.Json)]
        public string GetValidSymbols()
        {
            string json = string.Empty;
            string constr = ConfigurationManager.ConnectionStrings["SymbolContext"].ToString();

            using (SqlConnection conn = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM TickerSymbols WHERE ValidData = 1", conn))
                {
                    conn.Open();

                    TickerSymbols ts = new TickerSymbols();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TickerSymbol t = new TickerSymbol
                            {
                                Id = System.Convert.ToInt32(reader["Id"]),
                                Symbol = reader["Symbol"].ToString(),
                                Name = reader["Name"].ToString(),
                                ExchangeId = reader["ExchangeId"].ToString(),
                                Industry = reader["Industry"].ToString(),
                                Sector = reader["Sector"].ToString(),
                                Date = System.Convert.ToDateTime(reader["Date"].ToString()),
                                TimeStamp =  System.Convert.ToDateTime(reader["TimeStamp"].ToString()),
                                ValidData = reader["ValidData"].ToString() == "1" ? true : false,
                                KeyStat = reader["KeyStat"].ToString()
                                //,
                                //History = reader["History"].ToString() == "1" ? true : false,
                                //LastHistoryDate = System.Convert.ToDateTime(reader["LastHistoryDate"].ToString())
                            };

                            ts.Tickersymbols.Add(t);
                        }
                    }
                    json = JsonConvert.SerializeObject(ts);
                }
            }

            return json;
        }

        [WebGet(UriTemplate = "/Symbols", ResponseFormat = WebMessageFormat.Json)]
        public string GetSymbols()
        {
            Log.WriteLog(new LogEvent("SymbolService - GetSymbols loading...", ""));

            string json = string.Empty;

            try
            {
                var sectorList = GetSectorsAndIndustries();

                var xml = new XmlDocument();
                xml.LoadXml(sectorList);

                var assfart = JsonConvert.SerializeObject(xml);

                var jumba = assfart.Substring(assfart.IndexOf("results\":") + "results\":".Length);
                jumba = jumba.Substring(0, jumba.IndexOf("},\"#comment\":[]}"));
                
                var bege = JsonConvert.DeserializeObject<Sectors>(jumba);

                var symbolList = db.TickerSymbols.ToList();

                if (symbolList.Count > 0)
                {
                    json = JsonConvert.SerializeObject(symbolList);
                    // TODO: check date for stale data
                }
                else
                {
                    Log.WriteLog(new LogEvent("SymbolService - GetSymbols", "No data returned."));
                    
                    json = GetSymbolList();
                    
                    DataSet ds = DeSerializationToDataSet(json);
                    DataTable industryTable = ds.Tables["industry"];
                    DataTable companyTable = ds.Tables["company"];

                    DataView dv = new DataView(companyTable);
                    dv.Sort = "symbol asc";

                    DataTable sortedTable = dv.ToTable();

                    Dictionary<string, string> dic = new Dictionary<string, string>();

                    foreach (DataRow item in industryTable.Rows)
                    {
                        dic.Add(item["industry_id"].ToString(), item["name"].ToString());
                    }

                    TickerSymbol lastTicker = new TickerSymbol();

                    // TODO: add bulk copy
                    foreach (DataRow item in sortedTable.Rows)
                    {
                        string name = item["name"].ToString();
                        string symbol = item["symbol"].ToString();

                        TickerSymbol ts = new TickerSymbol
                            {
                                Symbol = symbol,
                                Name = name,
                                Date = DateTime.Now,
                                TimeStamp = DateTime.Now,
                                Industry = dic[item["industry_id"].ToString()]
                            };

                        if( lastTicker.Symbol == ts.Symbol && lastTicker.Name == ts.Name && lastTicker.Industry == ts.Industry )
                            continue;

                        lastTicker = ts;

                        db.TickerSymbols.Add(ts);
                        db.SaveChanges();
                    }

                    return json;
                }

            }
            catch (Exception ex)
            {
                return "Error doing other shit: " + ex.Message + " --- " + ex.InnerException;

            }
            return json;
        }

        [WebGet(UriTemplate = "/Peeps", ResponseFormat = WebMessageFormat.Json)]
        public string GetPeople()
        {
            string json = JsonConvert.SerializeObject(people);

            return json;
        }

        [WebGet(UriTemplate = "/Person?id={id}", ResponseFormat = WebMessageFormat.Json)]
        public string GetPerson(int id)
        {
            Person peep = people.SingleOrDefault(p => p.Id == id);
            string json = JsonConvert.SerializeObject(peep);

            return json;
        }

        [WebInvoke(UriTemplate = "Person", RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public Person InsertPerson(Person[] person)
        {
            return people[2];
        }

        [WebInvoke(UriTemplate = "/Person?id={id}", Method = "PUT")]
        public Person UpdatePerson(int id, Person person)
        {
            return people[1];
        }

        [WebInvoke(UriTemplate = "/Person?id={id}", Method = "DELETE")]
        public void DeletePerson(int id)
        {
        }

        private static string GetSectorsAndIndustries()
        {
            string url = string.Format("http://query.yahooapis.com/v1/public/yql?{0}",
                                       "q=select%20*%20from%20yahoo.finance.sectors&env=store://datatables.org/alltableswithkeys");
            string webData = string.Empty;
                try
            {
                using (WebClient client = new WebClient())
                {
                    webData = client.DownloadString(url);
                    webData = webData.Replace("&", "&amp;");

                    if (webData.Length < 500)
                    {
                        webData = "";
                        using (StreamReader sr = new StreamReader(@"D:\Projects\ScreenScraper\ScreenScraper\sectorlist.xml"))
                        {
                            webData = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("Error reading XML. Error: {0}", ex.Message));
                webData = string.Format("Error reading XML. Error: {0}", ex.Message);
            }
            return webData;
        }


        private static string GetSymbolList()
        {
            string url = string.Format(
                "http://query.yahooapis.com/v1/public/yql?q={0}", "select%20*%20from%20yahoo.finance.industry%20where%20id%20in%20(select%20industry.id%20from%20yahoo.finance.sectors)&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            string webData = string.Empty;

            try
            {
                using (WebClient client = new WebClient())
                {
                    webData = client.DownloadString(url);
                    webData = Regex.Replace(webData, @"[^\u0000-\u007F]", string.Empty).Replace("&", "&amp;");
                    if (webData.Length < 500)
                    {
                        webData = "";
                        using (StreamReader sr = new StreamReader(@"D:\Projects\ScreenScraper\ScreenScraper\symbolslist.xml"))
                        {
                            webData = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("Error reading XML. Error: {0}", ex.Message));
                webData = string.Format("Error reading XML. Error: {0}", ex.Message);
            }

            return webData;
        }

        private static DataSet DeSerializationToDataSet(string data)
        {
            DataSet deSerializeDS = new DataSet();
            try
            {
                using (TextReader theReader = new StringReader(data))
                {
                    deSerializeDS.ReadXml(theReader);
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("Error desearializing XML. Error: {0}", ex.Message));
            }
            return deSerializeDS;
        }

        private static void WriteLog(string message)
        {
            using (StreamWriter log = File.AppendText("logs/log.txt"))
            {
                log.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + message + "\r\n");
                log.Flush();
                log.Close();
            }
        }

        #region private objects
        private Person[] people = new Person[]
            {
                new Person
                    {
                        Id = 1,
                        FirstName = "Biff",
                        LastName = "McGillicutty"
                    },
                new Person
                    {
                        Id = 2,
                        FirstName = "Muffy",
                        LastName = "McSplit"
                    },
                new Person
                    {
                        Id = 3,
                        FirstName = "Stan",
                        LastName = "Standard"
                    }
            };

        #endregion private objects

    }
}