using Jil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Adzerk.Models
{
    public class Campaign
    {
        public long Id { get; set; }
        public long AdvertiserId { get; set; }
        public long? SalespersonId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<Flight> Flights { get; set; }
        public string CustomFieldsJson { get; set; }

        public CampaignDTO ToDTO()
        {
            var c = new CampaignDTO();

            c.Id = Id;
            c.AdvertiserId = AdvertiserId;
            c.SalespersonId = SalespersonId;
            c.Name = Name;
            c.IsDeleted = IsDeleted;
            c.IsActive = IsActive;
            c.Price = Price;
            c.CustomFieldsJson = CustomFieldsJson;

            c.StartDate = StartDate.ToUniversalTime().ToString("M/d/yyyy");
            if (EndDate.HasValue)
            {
                c.EndDate = EndDate.Value.ToUniversalTime().ToString("M/d/yyyy");
            }

            c.Flights = Flights.Select(f => f.ToDTO());

            return c;
        }
    }

    public class AdzerkDateTimeHelpers
    {
        public static DateTime ParseAdzerkDate(string str)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            if(!DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.AssumeUniversal, out date))
            {
                long ms = 0;

                // /Date(1293858000000)/
                if(!long.TryParse(str.Substring(6, 13), out ms))
                {
                    // \/Date(1293858000000-0500)\/
                    long.TryParse(str.Substring(7, 13), out ms);
                }

                return date.AddMilliseconds(ms);
            }

            return date;
        }
    }

    public class CampaignDTO
    {
        public long Id;
        public long AdvertiserId;
        public long? SalespersonId;
        public string Name;
        public bool IsDeleted;
        public bool IsActive;
        public decimal Price;
        public string StartDate;
        public string EndDate;
        public IEnumerable<FlightDTO> Flights;
        public string CustomFieldsJson;

        public Campaign ToCampaign()
        {
            var c = new Campaign();

            c.Id = Id;
            c.AdvertiserId = AdvertiserId;
            c.SalespersonId = SalespersonId;
            c.Name = Name;
            c.IsDeleted = IsDeleted;
            c.IsActive = IsActive;
            c.Price = Price;
            c.CustomFieldsJson = CustomFieldsJson;

            c.StartDate = AdzerkDateTimeHelpers.ParseAdzerkDate(StartDate);
            if (EndDate != null)
            {
                c.EndDate = AdzerkDateTimeHelpers.ParseAdzerkDate(EndDate);
            }

            if (Flights == null)
            {
                c.Flights = new List<Flight>();
            }
            else
            {
                c.Flights = Flights.Select(f => f.ToFlight());
            }

            return c;
        }
    }
}
