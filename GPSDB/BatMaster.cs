using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("batmaster")]
    public class BatMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("batmasterid")]
        public int BatMasterId { get; set; }
        [Column("batcreatedby")]
        public int BatCreatedBy { get; set; }
        [Column("batapprovedby")]
        public int BatApprovedBy { get; set; }
        [Column("batcreatedon")]
        public DateTime BatCreatedOn { get; set; }
        [Column("batapprovedon")]
        public DateTime BatApprovedOn { get; set; }
        [Column("batfilename")]
        public string BatFileName { get; set; }
        [Column("createdon")]
        public DateTime CreatedOn { get; set; }
    }

    [Table("batdetails")]
    public class BatDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idbatdetails")]
        public int BatDetailsId { get; set; }
        [Column("batmasterid")]
        public int BatMasterId { get; set; }
        [Column("statementtype")]
        public string StatementType { get; set; }
        [Column("statement")]
        public string Statement{ get; set; }
        [Column("createdon")]
        public DateTime CreatedOn { get; set; }
    }
}
