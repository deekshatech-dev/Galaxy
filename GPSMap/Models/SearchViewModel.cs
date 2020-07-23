using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class SearchViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string Ids { get; set; }
        public string KPI { get; set; }

        public IEnumerable<int> Id
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    return JsonConvert.DeserializeObject<List<int>>(Ids);
                }
                return Enumerable.Empty<int>();
            }
        }
        public List<Rights> rights { get; set; }
    }
}