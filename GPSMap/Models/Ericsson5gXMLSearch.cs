using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class Ericsson5gXMLSearch
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string MOClass { get; set; }
        public string ManagedElement { get; set; }
        public string counter { get; set; }

    }
}