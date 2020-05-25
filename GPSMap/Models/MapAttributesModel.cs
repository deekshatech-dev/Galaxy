using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class MapAttributesModel
    {
        public int AttributeId { get; set; }
        public string ClassId { get; set; }
        public string Name { get; set; }
        // public string tm_name { get; set; }
        public string IMEI { get; set; }
        public string IMSI { get; set; }
        public string Ue_Label { get; set; }
        public string Ue_Name { get; set; }
        public string Location { get; set; }
    }
}