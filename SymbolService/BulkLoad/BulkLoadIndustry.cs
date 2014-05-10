using System.Collections.Generic;
using System.Data;
using SymbolService.Models;
using SymbolService.Models.BulkLoad;

namespace SymbolService.BulkLoad
{
    public class BulkLoadIndustry : BaseBulkLoad
    {
        private static readonly string[] ColumnNames = new string[]
            {
                "Date", "DebtToEquity", "DivYieldPerCent",
                "MarketCap", "Name", "NetProfitMarginMrq", "OneDayPriceChgPerCent", "PriceToBook", "PriceToEarnings",
                "PriceToFreeCashFlowMrq", "ROEPerCent", "SectorId"
            };

        public BulkLoadIndustry() : base(ColumnNames)
        {

        }

        public DataTable LoadDataTableWithIndustries(IEnumerable<Industry> dStats, DataTable dt)
        {
            foreach (var value in dStats)
            {
                var sValue = value.Date + "^" + value.DebtToEquity + "^"
                             + value.DivYieldPerCent + "^" + value.MarketCap
                             + "^" + value.Name + "^" + value.NetProfitMarginMrq + "^" + value.OneDayPriceChgPerCent
                             + "^" + value.PriceToBook + "^" + value.PriceToEarnings + "^" +
                             value.PriceToFreeCashFlowMrq
                             + "^" + value.ROEPerCent + "^" + value.SectorId;

                DataRow row = dt.NewRow();

                row.ItemArray = sValue.Split('^');

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}