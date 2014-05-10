using System.Configuration;
using System.Data.Entity;
using SymbolService.Logs;

namespace SymbolService.Models.Context
{
    public class SymbolContext : DbContext
    {
        public DbSet<TickerSymbol> TickerSymbols { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<LastUpdate> LastUpdates { get; set; }
        public DbSet<LogEvent> Logs { get; set; }

        #region GetConnectionstring
        /// <summary>
        /// method to retrieve connection stringed in the web.config file
        /// </summary>
        /// <param name="str">Name of the connection</param>
        /// <remarks>Need a reference to the System.Configuration Namespace</remarks>
        /// <returns></returns>
        public static string GetConnectionString(string str)
        {
            //variable to hold our return value
            string conn = string.Empty;
            //check if a value was provided
            if (!string.IsNullOrEmpty(str))
            {
                //name provided so search for that connection
                conn = ConfigurationManager.ConnectionStrings[str].ConnectionString;
            }
            else
            //name not provided, get the 'default' connection
            {
                conn = ConfigurationManager.ConnectionStrings["SymbolContext"].ConnectionString;
            }
            //return the value
            return conn;
        }
        #endregion 
    }
}
