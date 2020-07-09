using GPSDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class EricssonGV
    {
        public string MOC { get; set; }
        public string PN { get; set; }
    }

    public class xmlDataRawEricsson
    {
        public List<ericssonxmldetail> xmlDataEricsson { get; set; }
    }
}