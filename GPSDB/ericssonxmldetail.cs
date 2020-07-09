using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("ericssonxmldetail")]
    public class ericssonxmldetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int DetaikID { get; set; }
        public int ParseID { get; set; }
        public string SubNetwork { get; set; }
        public string MeContext { get; set; }
        public string ManagedElementId { get; set; }
        public string DataContainer { get; set; }
        public string UeMeasControlId { get; set; }
        public string MOC { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
        public string GUIValue { get; set; }
        public string Discrepency { get; set; }
        public string NewValue { get; set; }
        public string Schedule { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? BatchId { get; set; }

        
    }
}
