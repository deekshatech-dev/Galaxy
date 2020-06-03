using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace GPSDB
{
    [Table("kpimaster")]
    public class KPIMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KPIId { get; set; }
        public string KPIName { get; set; }
        public bool IsActive { get; set; }
    }
}
