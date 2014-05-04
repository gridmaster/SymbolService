using System.Collections.Generic;
using SymbolService.Models.Context;

namespace SymbolService.Logs
{
    public class Log : List<LogEvent>
    {
        public int LogId { get; set; }

        public static void WriteLog(LogEvent le)
        {
            var db = new SymbolContext();

            db.Logs.Add(le);
            db.SaveChanges();
        }
    }
}