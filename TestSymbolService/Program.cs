using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestSymbolService.Models;

namespace TestSymbolService
{
    class Program
    {
        private static string BaseUri = @"http://localhost:45667"; // @"http://tickersymbol.info"; 
        private static string LoadSectorUri = @"/LoadSectors";
        private static string PersonUri = @"/Person";

        private static Person[] people = new Person[]
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

        static void Main(string[] args)
        {

            // var result = Post(BaseUri + PersonUri, people);

            string uri = BaseUri + LoadSectorUri;

            List<Sector> sectors = new List<Sector>();
            
            Industry industry1 = new Industry
                {
                    Id = 12,
                    IndustryId = 1,
                    Name = "hoo-doo"
                };
            Industry industry2 = new Industry
            {
                Id = 13,
                IndustryId = 2,
                Name = "voo-doo"
            };
            
            List<Industry> industries = new List<Industry> {industry1, industry2};

            Sector sector = new Sector
                {
                    id = 1,
                    name = "Sup dude?",
                    industry = industries
                };
            sectors.Add(sector);
            sectors.Add(sector);

            SectorRequest sr = new SectorRequest
                {
                    sector = sectors
                };
            sr.token = "bc2afdc0-6f68-497a-9f6c-4e261331c256";

            string jsondata = JsonConvert.SerializeObject(sr);
            var json = Post(uri, sr);
            Console.WriteLine(json);

            sr.token = Guid.NewGuid().ToString();
            jsondata = JsonConvert.SerializeObject(sr);

            json = Post(uri, sr);
            Console.WriteLine(json);
            Console.ReadKey();
        }

        private static string Post<T>(string uri, T postData)
        {
            string jsonData = string.Empty;
            const string applicationType = "application/json"; // "application/x-www-form-urlencoded"; // 

            try
            {
                jsonData = JsonConvert.SerializeObject(postData);
                var fuckit = JsonConvert.DeserializeObject<T>(jsonData);

                byte[] requestData = Encoding.UTF8.GetBytes(jsonData);

                WebRequest request = WebRequest.Create(uri);
                request.Method = "POST";
                request.ContentType = applicationType;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(requestData, 0, requestData.Length);
                dataStream.Dispose();

                string jsonResponse = string.Empty;
                using (WebResponse response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusDescription == "OK")
                    {
                        dataStream = response.GetResponseStream();
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            jsonResponse = reader.ReadToEnd();
                        }
                    }
                }

                //BasicResponseData responseCode = JsonConvert.DeserializeObject<BasicResponseData>(jsonResponse);
                //if (responseCode.error > 0)
                //{
                //    throw new Exception(string.Format("Error CodeString {0} Returned from Web Service call.",
                //                                      responseCode.error));
                //}

                return jsonResponse;
            }
            catch (Exception ex)
            {
                string error = string.Format(
                    "Exception in postAsync{0}URI: {1}{0}Post Data: {2}{0}Content Type: {3}{0}",
                    Environment.NewLine,
                    uri,
                    jsonData,
                    applicationType);

                throw new Exception(error, ex);
            }
        }

    }
}
