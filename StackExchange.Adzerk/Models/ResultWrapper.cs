using System.Collections.Generic;

namespace StackExchange.Adzerk.Models
{
    public class ResultWrapper<T>
    {
        public int pageNumber;
        public int pageSize;
        public int totalPages;
        public int totalItems;

        public IEnumerable<T> items;
    }
}
