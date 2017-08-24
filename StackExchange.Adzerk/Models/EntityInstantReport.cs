using System.Collections.Generic;

namespace StackExchange.Adzerk.Models
{
    internal class InstantReportBulkRequest
    {
        public IEnumerable<long> Flights { get; set; }
    }

    internal class InstantReportResponse
    {
        public Dictionary<string, EntityInstantReport> Flights { get; set; }
    }

    public class EntityInstantReport
    {
        public long Impressions { get; set; }
        public long Clicks { get; set; }
        public long Conversions { get; set; }
        public double Revenue { get; set; }
    }
}
