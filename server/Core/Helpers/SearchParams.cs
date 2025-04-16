using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class SearchParams : PaginationParams
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
}
