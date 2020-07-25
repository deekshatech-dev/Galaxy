using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class Rights
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RightsId { get; set; }
        public string RightsName { get; set; }
        public string IconName { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

    public class RightsModel
    {
        public List<Rights> rights { get; set; }
    }
}