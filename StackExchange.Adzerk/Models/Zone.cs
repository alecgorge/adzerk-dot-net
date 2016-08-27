using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Adzerk.Models
{
    public class Zone
    {
        public long Id { get; set; }
        public long SiteId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
