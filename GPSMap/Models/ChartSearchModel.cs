using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class ChartSearchModel
    {
        public DateTime? ForDate { get; set; }

        public string KPIName { get; set; }

        public string Trend { get; set; }
    }
}