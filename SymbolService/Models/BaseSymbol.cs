using System.Globalization;
using System.Runtime.Serialization;

namespace SymbolService.Models
{
    public class BaseSymbol
    {
        protected string dateForSerialization;

        //[OnSerializing]
        //void OnSerializing(StreamingContext context)
        //{
        //    this.dateForSerialization = this.Date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        //}

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            this.dateForSerialization = "1900-01-01";
        }
    }
}