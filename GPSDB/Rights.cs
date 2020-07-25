using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSDB
{
    [Table("tt_rightsItems")]
    public class RightsItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("tt_RightsId")]
        public int RightsId{ get; set; }
        [Column("tt_RightsName")]
        public string RightsName { get; set; }
        public string IconName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

    [Table("tt_userrights")]
    public class UserRights
    {
        [Key, Column("userid", Order = 0)]
        public int UserId { get; set; }
        [Key, Column("tt_RightsId", Order = 1)]
        public int RightsId { get; set; }
        public bool Read{ get; set; }
        public bool Write { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
