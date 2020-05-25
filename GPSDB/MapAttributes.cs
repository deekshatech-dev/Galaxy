using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("tt_attribute_value")]
    public class MapAttributes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int attributeid { get; set; }
        public string class_id { get; set; }
        public string name { get; set; }
        // public string tm_name { get; set; }
        public string imei { get; set; }
        public string imsi { get; set; }
        public string ue_label { get; set; }
        public string ue_name { get; set; }
        public string location { get; set; }
        // public string ue_location { get; set; }
    }
}
