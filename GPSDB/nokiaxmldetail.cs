using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("nokiaxmldetail")]
    public class nokiaxmldetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int DetaikID { get; set; }
        public int ParseID { get; set; }
        public string ClassName { get; set; }
        public string DistName { get; set; }
        public string ManagedObject { get; set; }
        public string Id { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
        public string GUIValue { get; set; }
        public string Discrepency { get; set; }
        public string Schedule { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? BatchId { get; set; }





    }
}