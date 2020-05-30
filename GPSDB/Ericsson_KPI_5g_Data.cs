using System;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("ericsson_kpi_5g_data")]
    public class Ericsson_KPI_5g_Data
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int mstid { get; set; }
        public string MOClass { get; set; }
        public string MOClassValue { get; set; }
        public string ManagedElement { get; set; }
        public int CellNameID { get; set; }
        public string CellName { get; set; }
        public string FrequencyBand { get; set; }
        public string KPIName { get; set; }
        public string KPIValue { get; set; }
        public string Numerator { get; set; }
        public string Denominator { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
