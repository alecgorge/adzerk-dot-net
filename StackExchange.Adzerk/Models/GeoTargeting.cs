namespace StackExchange.Adzerk.Models
{
    public class GeoTargeting
    {
        public long LocationId { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public int MetroCode { get; set; }
        public bool IsExclude { get; set; }
    }
}
