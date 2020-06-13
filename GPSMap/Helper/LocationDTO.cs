using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Helper
{
	public class LocationDTO
	{
		public int id { get; set; }

		public int object_id { get; set; }

		public int session_id { get; set; }

		public byte[] kpiLTE_UE_GPS { get; set; }

		public string strGPSFromSimpleStr { get; set; }

		public string strGPSFromB64Str { get; set; }

		public string strGPSFromUTF8Str { get; set; }

	}

	public class CQIData
	{


		public int objectid { get; set; }
		public string LTE_UE_GPS_longitude { get; set; }
		public string LTE_UE_GPS_latitude { get; set; }
		public string LTE_UE_CQI_CQI { get; set; }
		public DateTime period_from { get; set; }
		public DateTime period_to { get; set; }

	}


	public class RSRPData
	{
		public string LTE_UE_GPS_longitude { get; set; }
		public string LTE_UE_GPS_latitude { get; set; }
		public string LTE_UE_RSRP_RSRP { get; set; }
		public DateTime period_from { get; set; }
		public DateTime period_to { get; set; }
	}

	public class RSSNRData
	{
		public string LTE_UE_GPS_longitude { get; set; }
		public string LTE_UE_GPS_latitude { get; set; }
		public string LTE_UE_RSSNR_RSSNR { get; set; }
		public DateTime period_from { get; set; }
		public DateTime period_to { get; set; }
	}

	public class CoOrdinateInfo
	{
		public string sLongitude { get; set; }
		public string sLatitude { get; set; }

		public string sCQI { get; set; }
	}

	public class TreeViewNode
	{
		public string id;
		public string parentid;
		public string text;
		public string href;
		public bool isDirectory;
		public string path;
		public List<string> tags;
		public List<TreeViewNode> nodes;
	}

}