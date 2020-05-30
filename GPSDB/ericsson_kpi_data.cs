using System;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GPSDB
{
    [Table("ericsson_kpi_data")]
    public class Ericsson_KPI_Data
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransID { get; set; }
        public string ManagedElement { get; set; }
        public string KPIName { get; set; }
        public string KPIValue { get; set; }
        public decimal Numerator { get; set; }
        public decimal Denominator { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
