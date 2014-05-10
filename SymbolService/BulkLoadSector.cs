using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using SymbolService.Logs;
using SymbolService.Models;

namespace SymbolService
{
    public static class BulkLoadSector
    {
        private static readonly string[] ColumnNames = new string[] { "Date", "DebtToEquity", "DivYieldPerCent", 
            "MarketCap", "Name", "NetProfitMarginMrq", "OneDayPriceChgPerCent", "PriceToBook", "PriceToEarnings", "PriceToFreeCashFlowMrq", "ROEPerCent" };

        public static DataTable ConfigureDataTableForSectors()
        {
            var dt = new DataTable();

            for (int i = 0; i < ColumnNames.Length; i++)
            {
                dt.Columns.Add(new DataColumn());
                dt.Columns[i].ColumnName = ColumnNames[i];
            }
            return dt;
        }

        public static DataTable LoadDataTableWithSectors(IEnumerable<Sector> dStats, DataTable dt)
        {
            foreach (var value in dStats)
            {
                var sValue = value.Date + "^" + value.DebtToEquity + "^"
                             + value.DivYieldPerCent + "^" + value.MarketCap
                             + "^" + value.Name + "^" + value.NetProfitMarginMrq + "^" + value.OneDayPriceChgPerCent
                             + "^" + value.PriceToBook + "^" + value.PriceToEarnings + "^" + value.PriceToFreeCashFlowMrq + "^" + value.ROEPerCent;

                DataRow row = dt.NewRow();

                row.ItemArray = sValue.Split('^');

                dt.Rows.Add(row);
            }

            return dt;
        }

        public static void BulkCopySectors(DataTable dt)
        {

            // get connection string
            Configuration rootWebConfig =
                WebConfigurationManager.OpenWebConfiguration("/MyWebSiteRoot");

            ConnectionStringSettings connString =
                rootWebConfig.ConnectionStrings.ConnectionStrings["SymbolContext"];

            //        private static string[] ColumnNames = new string[] { "Date", "DebtToEquity", "DivYieldPerCent", 
            //"MarketCap", "Name", "NetProfitMarginMrq", "OneDayPriceChgPerCent", "PriceToBook", "PriceToEarnings", "PriceToFreeCashFlowMrq", "ROEPerCent" };

            // use bulk copy
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connString.ToString()))
            {
                bulkCopy.ColumnMappings.Add(0, "Date");
                bulkCopy.ColumnMappings.Add(1, "DebtToEquity");
                bulkCopy.ColumnMappings.Add(2, "DivYieldPerCent");
                bulkCopy.ColumnMappings.Add(3, "MarketCap");
                bulkCopy.ColumnMappings.Add(4, "Name");
                bulkCopy.ColumnMappings.Add(5, "NetProfitMarginMrq");
                bulkCopy.ColumnMappings.Add(6, "OneDayPriceChgPerCent");
                bulkCopy.ColumnMappings.Add(7, "PriceToBook");
                bulkCopy.ColumnMappings.Add(8, "PriceToEarnings");
                bulkCopy.ColumnMappings.Add(9, "PriceToFreeCashFlowMrq");
                bulkCopy.ColumnMappings.Add(10, "ROEPerCent");
                bulkCopy.BulkCopyTimeout = 600; // in seconds 
                bulkCopy.DestinationTableName = "Sectors"; 
                try
                {
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(new LogEvent("BulkLoadSector - BulkCopySectors()", "Bulk load error: " + ex.Message));
                }
                bulkCopy.Close();
            }
        }
    }
}