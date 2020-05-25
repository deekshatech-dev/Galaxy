using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("ericsson_5gmaster")]
   public class ericsson_5gmaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int mstid { get; set; }
        public string formatversion { get; set; }
        public string vendorname { get; set; }
        public string dgprefix { get; set; }
        public string HeaderTime { get; set; }
        public string FooterTime { get; set; }
        public string Countercount { get; set; }
        public string xmltype { get; set; }
        public string MOClass { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        
    }
}

