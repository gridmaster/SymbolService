﻿using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SymbolService.Models
{
    public class Sector : BaseSymbol
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "OneDayPriceChgPerCent")]
        public decimal OneDayPriceChgPerCent { get; set; }
        [JsonProperty(PropertyName = "MarketCap")]
        public string MarketCap { get; set; }
        [JsonProperty(PropertyName = "PriceToEarnings")]
        public decimal PriceToEarnings { get; set; }
        [JsonProperty(PropertyName = "ROEPerCent")]
        public decimal ROEPerCent { get; set; }
        [JsonProperty(PropertyName = "DivYieldPerCent")]
        public decimal DivYieldPerCent { get; set; }
        [JsonProperty(PropertyName = "DebtToEquity")]
        public decimal DebtToEquity { get; set; }
        [JsonProperty(PropertyName = "PriceToBook")]
        public decimal PriceToBook { get; set; }
        [JsonProperty(PropertyName = "NetProfitMarginMrq")]
        public decimal NetProfitMarginMrq { get; set; }
        [JsonProperty(PropertyName = "PriceToFreeCashFlowMrq")]
        public decimal PriceToFreeCashFlowMrq { get; set; }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            base.dateForSerialization = this.Date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
    }
}