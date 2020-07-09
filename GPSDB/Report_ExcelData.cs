using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("tt_report_exceldata")]
    public class Report_ExcelData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportExcelDataId { get; set; }
        public string Object { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string CLUSTER_NAME { get; set; }
        public decimal? PLMN_RRC_estab_succ_rate { get; set; }
        public decimal? PLMN_RRC_estab_attempt { get; set; }
        public decimal? PLMN_S1_estab_succ_rate { get; set; }
        public decimal? PLMN_S1_estab_attempt { get; set; }
        public decimal? PLMN_ERAB_init_estab_succ_rate { get; set; }
        public decimal? PLMN_ERAB_init_estab_attempt { get; set; }
        public decimal? PLMN_ERAB_add_estab_succ_rate { get; set; }
        public decimal? PLMN_ERAB_add_estab_attempt { get; set; }
        public decimal? PLMN_ERAB_estab_succ_rate { get; set; }
        public decimal? PLMN_ERAB_estab_attempt { get; set; }
        public decimal? PLMN_VoLTE_init_estab_succ_rate { get; set; }
        public decimal? PLMN_VoLTE_init_estab_attempt { get; set; }
        public decimal? PLMN_VoLTE_add_estab_succ_rate { get; set; }
        public decimal? PLMN_VoLTE_add_estab_attempt { get; set; }
        public decimal? PLMN_VoLTE_estab_succ_rate { get; set; }
        public decimal? PLMN_VoLTE_estab_attempt { get; set; }
        public decimal? PLMN_ERAB_init_accessibility { get; set; }
        public decimal? PLMN_ERAB_init_access_attempt { get; set; }
        public decimal? PLMN_ERAB_add_accessibility { get; set; }
        public decimal? PLMN_ERAB_add_access_attempt { get; set; }
        public decimal? PLMN_ERAB_accessibility { get; set; }
        public decimal? PLMN_ERAB_access_attempt { get; set; }
        public string PLMN_VoLTE_accessibility { get; set; }
        public decimal? PLMN_VoLTE_access_attempt { get; set; }
        public decimal? PLMN_ERAB_normal_release { get; set; }
        public decimal? PLMN_ERAB_release { get; set; }
        public decimal? PLMN_ERAB_retainability { get; set; }
        public decimal? PLMN_VoLTE_normal_release { get; set; }
        public decimal? PLMN_VoLTE_release { get; set; }
        public decimal? PLMN_VoLTE_retainability { get; set; }
        public decimal? PLMN_HO_intra_eNodeB_succ_rate { get; set; }
        public decimal? PLMN_HO_intra_eNodeB_attempt { get; set; }
        public decimal? PLMN_HO_inter_eNodeB_succ_rate { get; set; }
        public decimal? PLMN_HO_inter_eNodeB_attempt { get; set; }
        public decimal? PLMN_HO_intra_freq_succ_rate { get; set; }
        public decimal? PLMN_HO_intra_freq_attempt { get; set; }
        public decimal? PLMN_HO_inter_freq_succ_rate { get; set; }
        public decimal? PLMN_HO_inter_freq_attempt { get; set; }
        public decimal? PLMN_throughput_DL { get; set; }
        public decimal? PLMN_throughput_UL { get; set; }
        public decimal? PLMN_user_speed_DL { get; set; }
        public decimal? PLMN_user_speed_UL { get; set; }
        public decimal? PLMN_VoLTE_integrity_DL { get; set; }
        public decimal? PLMN_VoLTE_sample_DL { get; set; }
        public decimal? PLMN_VoLTE_integrity_UL { get; set; }
        public decimal? PLMN_VoLTE_sample_UL { get; set; }
        public decimal? PLMN_VoLTE_FER_DL { get; set; }
        public decimal? PLMN_VoLTE_packet_DL { get; set; }
        public decimal? PLMN_VoLTE_FER_UL { get; set; }
        public decimal? PLMN_VoLTE_packet_UL { get; set; }

    }
}
