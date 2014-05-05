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
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using SymbolService.Logs;
using SymbolService.Models;
using SymbolService.Models.Context;

namespace RESTSymbolService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SymbolService : ISymbolService
    {
        private SymbolContext db = new SymbolContext();

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "/LoadSectors")]
        public string LoadSectors(SectorRequest sectors)
        {
            string result = string.Empty;

            if (sectors.token != "bc2afdc0-6f68-497a-9f6c-4e261331c256")
            {
                result = "You didn't say the magic word!";
            }
            else
            {
                result = "Got 'em!";
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