using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPSDB
{
    [Table("tt_kpidata")]
    public class MapPoints
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? attribute_id { get; set; }
        public int? object_id { get; set; }
        public int? session_id { get; set; }
        public DateTime period_from { get; set; }
        public DateTime period_to { get; set; }
        public string LTE_UE_GPS_longitude { get; set; }
        public string LTE_UE_GPS_latitude { get; set; }
        public string LTE_UE_CQI_CQI { get; set; }
        public string LTE_UE_RSRP_RSRP { get; set; }
        public string LTE_UE_RSSNR_RSSNR { get; set; }

        public string LTE_UE_GPS_height { get; set; }
        public string LTE_UE_SERVER_RAT { get; set; }
        public string LTE_UE_SERVER_DUPLEX { get; set; }
        public string LTE_UE_SERVER_BAND { get; set; }
        public string LTE_UE_SERVER_CARRIER { get; set; }
        public string LTE_UE_SERVER_FREQUENCY { get; set; }
        public string LTE_UE_SERVER_ENODEB { get; set; }
        public string LTE_UE_SERVER_CELL { get; set; }
        public string LTE_UE_SERVER_PCI { get; set; }
        public string LTE_UE_RSRP_N { get; set; }
        public string LTE_UE_RSRQ_RSRQ { get; set; }
        public string LTE_UE_RSRQ_N { get; set; }
        public string LTE_UE_L_L { get; set; }
        public string LTE_UE_L_N { get; set; }
        public string LTE_UE_IC_IC { get; set; }
        public string LTE_UE_IC_N { get; set; }
        public string LTE_UE_RSSNR_N { get; set; }
        public string LTE_UE_NF_UE_NF { get; set; }
        public string LTE_UE_NF_UE_NF_N { get; set; }
        public string LTE_UE_CQI_N { get; set; }
        public string LTE_UE_RI_RI { get; set; }
        public string LTE_UE_RI_N { get; set; }
        public string LTE_UE_UTILIZATION_DL_UTILIZAYION { get; set; }
        public string LTE_UE_UTILIZATION_DL_N { get; set; }
        public string LTE_UE_TX_POWER_PUSCH_TXPOWER { get; set; }
        public string LTE_UE_TX_POWER_PUSCH_N { get; set; }
        public string LTE_UE_TX_POWER_PUCCH_TXPOWER { get; set; }
        public string LTE_UE_TX_POWER_PUCCH_N { get; set; }
        public string LTE_UE_PHR_PHR { get; set; }
        public string LTE_UE_PHR_N { get; set; }
        public string LTE_UE_TA_TA { get; set; }
        public string LTE_UE_TA_N { get; set; }
        public string LTE_UE_ATTENUATION_ATTENUATION { get; set; }
        public string LTE_UE_ATTENUATION_N { get; set; }
        public string LTE_UE_RSSI_PUSCH_RSSI { get; set; }
        public string LTE_UE_RSSI_PUSCH_N { get; set; }
        public string LTE_UE_RSSI_PUCCH_RSSI { get; set; }
        public string LTE_UE_RSSI_PUCCH_N { get; set; }
        public string LTE_UE_PSD_PUSCH_PSD { get; set; }
        public string LTE_UE_PSD_PUSCH_N { get; set; }
        public string LTE_UE_MAC_BW_DL_PRBs { get; set; }
        public string LTE_UE_MAC_BW_DL_N { get; set; }
        public string LTE_UE_MAC_BW_UL_PRBs { get; set; }
        public string LTE_UE_MAC_BW_UL_N { get; set; }
        public string LTE_UE_MAC_THROUGHPUT_DL_THROUGHPUT { get; set; }
        public string LTE_UE_MAC_THROUGHPUT_DL_N { get; set; }
        public string LTE_UE_MAC_THROUGHPUT_UL_THROUGHPUT { get; set; }
        public string LTE_UE_MAC_THROUGHPUT_UL_N { get; set; }
        public string LTE_UE_MAC_BLER_DL_BLER { get; set; }
        public string LTE_UE_MAC_BLER_DL_N { get; set; }
        public string LTE_UE_MAC_BLER_UL_BLER { get; set; }
        public string LTE_UE_MAC_BLER_UL_N { get; set; }
        public string LTE_UE_MAC_EFFICIENCY_DL_EFFICIENCY { get; set; }
        public string LTE_UE_MAC_EFFICIENCY_DL_N { get; set; }
        public string LTE_UE_MAC_EFFICIENCY_UL_EFFICIENCY { get; set; }
        public string LTE_UE_MAC_EFFICIENCY_UL_N { get; set; }
        public string LTE_UE_PDCP_THROUGHPUT_DL_THROUGHPUT { get; set; }
        public string LTE_UE_PDCP_THROUGHPUT_DL_N { get; set; }
        public string LTE_UE_PDCP_THROUGHPUT_UL_THROUGHPUT { get; set; }
        public string LTE_UE_PDCP_THROUGHPUT_UL_N { get; set; }
        public string LTE_UE_PDCP_BEARER_DL_BEARER { get; set; }
        public string LTE_UE_PDCP_BEARER_DL_N { get; set; }
        public string LTE_UE_PDCP_BEARER_UL_BEARER { get; set; }
        public string LTE_UE_PDCP_BEARER_UL_N { get; set; }
        public string LTE_UE_ATTACH_SUCCESS_N_ATTEMPT { get; set; }
        public string LTE_UE_ATTACH_SUCCESS_N_SUCCESS { get; set; }
        public string LTE_UE_ATTACH_FAILURE_N_ATTEMPT { get; set; }
        public string LTE_UE_ATTACH_FAILURE_N_FAILURE_NORESPONSE { get; set; }
        public string LTE_UE_ATTACH_FAILURE_N_FAILURE_REJECT { get; set; }
        public string LTE_UE_ATTACH_FAILURE_N_FAILURE_UE { get; set; }


    }
}