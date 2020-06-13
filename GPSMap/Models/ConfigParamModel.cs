using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class ConfigParamModel
    {
        public string Path { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string PLMNObjects { get; set; }
    }
}