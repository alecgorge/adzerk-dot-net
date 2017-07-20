namespace StackExchange.Adzerk.Models
{
    public class Site
    {
        public long Id { get; set; }
        public long? PublisherAccountId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
