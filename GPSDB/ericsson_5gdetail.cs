using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("ericsson_5gdetail")]
  public  class ericsson_5gdetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int detailId { get; set; }
        public int mstid { get; set; }
        public string MOClass { get; set; }
        public string MOClassValue { get; set; }
        public string JobID { get; set; }
        public string ManagedElement { get; set; }
        public string Equipment { get; set; }
        public string MOClass1 { get; set; }
        public string MOClass2 { get; set; }
        public string counter { get; set; }
        public string CounterValue { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CellName { get; set; }


    }
}
