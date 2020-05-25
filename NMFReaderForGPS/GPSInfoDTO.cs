using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMFReaderForGPS
{
	public class GPSInfoDTO
	{
		public string sTimeStemp { get; set; }
		public string sLongitude { get; set; }
		public string sLatitude { get; set; }

		public string sheight { get; set; }

		public string sCQI { get; set; }

		public string sRSRP { get; set; }
	}

	public class CalculatedGPSInfoDTO
	{
		public string object_id { get; set; }

		public string session_id { get; set; }

		public DateTime sTimeStemp { get; set; }

		/* GPS */
		public DateTime time_gps { get; set; }

		public string time_kpi { get; set; }

		public string LTE_UE_GPS_Longitude { get; set; }

		public string LTE_UE_GPS_Latitude { get; set; }
		

		public string LTE_UE_GPS_Height { get; set; }

		/* SERVER */

		public string LTE_UE_SERVER_RAT { get; set; }
		public string LTE_UE_SERVER_duplex { get; set; }
		public string LTE_UE_SERVER_band { get; set; }
		public string LTE_UE_SERVER_carrier { get; set; }
		public string LTE_UE_SERVER_frequency { get; set; }
		public string LTE_UE_SERVER_enodeb { get; set; }
		public string LTE_UE_SERVER_cell { get; set; }

		public string LTE_UE_SERVER_PCI { get; set; }

		/* RSRP*/

		public string LTE_UE_RSRP_RSRP { get; set; }

		public string LTE_UE_RSRP_N { get; set; }

		/* RSRQ*/

		public string LTE_UE_RSRQ_RSRQ { get; set; }

		public string LTE_UE_RSRQ_N { get; set; }

		/*L N*/

		public string LTE_UE_L_L { get; set; }

		public string LTE_UE_L_N { get; set; }

		/* IC */

		public string LTE_UE_IC_IC { get; set; }

		public string LTE_UE_IC_N { get; set; }


		/* RSSNR */

		public string LTE_UE_RSSNR_RSSNR { get; set; }

		public string LTE_UE_RSSNR_N { get; set; }


		/* NF_UE */

		public string LTE_UE_NF_UE_NF { get; set; }

		public string LTE_UE_NF_UE_N { get; set; }



		/* CQI */

		public string LTE_UE_CQI_CQI { get; set; }

		public string LTE_UE_CQI_N { get; set; }

		/*  UE_RI */

		public string LTE_UE_RI_RI { get; set; }

		public string LTE_UE_RI_N { get; set; }

		/*  UTILIZATION_DL */

		public string LTE_UE_UTILIZATION_DL_utilization { get; set; }

		public string LTE_UE_UTILIZATION_DL_N { get; set; }


		/*  POWER_PUSCH */

		public string LTE_UE_TX_POWER_PUSCH_TXpower { get; set; }

		public string LTE_UE_TX_POWER_PUSCH_N { get; set; }


		/*  POWER_PUCCH */

		public string LTE_UE_TX_POWER_PUCCH_TXpower { get; set; }

		public string LTE_UE_TX_POWER_PUCCH_N { get; set; }

		/*  UE_PHR */

		public string LTE_UE_PHR_PHR { get; set; }

		public string LTE_UE_PHR_N { get; set; }


		/*  UE_TA */

		public string LTE_UE_TA_TA { get; set; }

		public string LTE_UE_TA_N { get; set; }

		/*  UE_TA */

		public string LTE_UE_ATTENUATION_attenuation { get; set; }

		public string LTE_UE_ATTENUATION_N { get; set; }


		/*  RSSI_PUSCH */

		public string LTE_UE_RSSI_PUSCH_RSSI { get; set; }

		public string LTE_UE_RSSI_PUSCH_N { get; set; }



		/*  RSSI_PUCCH */

		public string LTE_UE_RSSI_PUCCH_RSSI { get; set; }

		public string LTE_UE_RSSI_PUCCH_N { get; set; }

		/* LTE_UE_PSD_PUSCH */

		public string LTE_UE_PSD_PUSCH_PSD { get; set; }

		public string LTE_UE_PSD_PUSCH_N { get; set; }



		/*  UE_MAC_BW_DL */

		public string LTE_UE_MAC_BW_DL_PRBs { get; set; }

		public string LTE_UE_MAC_BW_DL_N { get; set; }

		/*  UE_MAC_BW_UL */

		public string LTE_UE_MAC_BW_UL_PRBs { get; set; }

		public string LTE_UE_MAC_BW_UL_N { get; set; }



		/* UE_MAC_THROUGHPUT_DL */

		public string LTE_UE_MAC_THROUGHPUT_DL_throughput { get; set; }

		public string LTE_UE_MAC_THROUGHPUT_DL_N { get; set; }


		/* UE_MAC_THROUGHPUT_UL */

		public string LTE_UE_MAC_THROUGHPUT_UL_throughput { get; set; }

		public string LTE_UE_MAC_THROUGHPUT_UL_N { get; set; }

		/* LTE_UE_MAC_BLER_DL */
		
		public string LTE_UE_MAC_BLER_DL_BLER { get; set; }

		public string LTE_UE_MAC_BLER_DL_N { get; set; }


		/* LTE_UE_MAC_BLER_UL */

		public string LTE_UE_MAC_BLER_UL_BLER { get; set; }

		public string LTE_UE_MAC_BLER_UL_N { get; set; }

		/*  LTE_UE_MAC_EFFICIENCY_DL */
		public string LTE_UE_MAC_EFFICIENCY_DL_efficiency { get; set; }

		public string LTE_UE_MAC_EFFICIENCY_DL_N { get; set; }

		/*  LTE_UE_MAC_EFFICIENCY_UL */
		public string LTE_UE_MAC_EFFICIENCY_UL_efficiency { get; set; }

		public string LTE_UE_MAC_EFFICIENCY_UL_N { get; set; }

		/* UE_PDCP_THROUGHPUT_DL */

		public string LTE_UE_PDCP_THROUGHPUT_DL_throughput { get; set; }

		public string LTE_UE_PDCP_THROUGHPUT_DL_N { get; set; }


		/* UE_PDCP_THROUGHPUT_UL */

		public string LTE_UE_PDCP_THROUGHPUT_UL_throughput { get; set; }

		public string LTE_UE_PDCP_THROUGHPUT_UL_N { get; set; }


		/* UE_PDCP_BEARER_DL */

		public string LTE_UE_PDCP_BEARER_DL_bearer { get; set; }

		public string LTE_UE_PDCP_BEARER_DL_N { get; set; }


		/* UE_PDCP_BEARER_UL */

		public string LTE_UE_PDCP_BEARER_UL_bearer { get; set; }

		public string LTE_UE_PDCP_BEARER_UL_N { get; set; }


		/* LTE_UE_ATTACH_SUCCESS */

		public string LTE_UE_ATTACH_SUCCESS_N_attempt { get; set; }

		public string LTE_UE_ATTACH_SUCCESS_N_success { get; set; }

		/* LTE_UE_ATTACH_FAILURE */

		public string LTE_UE_ATTACH_FAILURE_N_attempt { get; set; }

		public string LTE_UE_ATTACH_FAILURE_N_failure_noresponse { get; set; }

		public string LTE_UE_ATTACH_FAILURE_N_failure_reject { get; set; }

		public string LTE_UE_ATTACH_FAILURE_N_failure_UE { get; set; }

		public string period_from { get; set; }
		public string period_to { get; set; }


	}


	/*
	if (lstGPSInfo != null && lstGPSInfo.Count > 0)
	{

		//SaveDataIntoDB(lstGPSInfo);

		StringBuilder sb = new StringBuilder();
		string finalPath = Path.Combine(parentOutPutPath, CSVOutPutFile + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");
		string header = "";
		var info = typeof(GPSInfoDTO).GetProperties();
		if (!File.Exists(finalPath))
		{
			var file = File.Create(finalPath);
			file.Close();
			foreach (var prop in typeof(GPSInfoDTO).GetProperties())
			{
				header += prop.Name + ", ";
			}
			header = header.Substring(0, header.Length - 2);
			sb.AppendLine(header);
			TextWriter sw = new StreamWriter(finalPath, true);
			sw.Write(sb.ToString());
			sw.Close();
		}
		foreach (var obj in lstGPSInfo)
		{
			sb = new StringBuilder();
			var line = "";
			foreach (var prop in info)
			{
				line += prop.GetValue(obj, null) + ", ";
			}
			line = line.Substring(0, line.Length - 2);
			sb.AppendLine(line);
			TextWriter sw = new StreamWriter(finalPath, true);
			sw.Write(sb.ToString());
			sw.Close();
		}

	}
	*/
}
