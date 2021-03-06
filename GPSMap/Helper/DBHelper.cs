﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace GPSMap.Helper
{
	public class DBHelper
	{
		// Private Connection string 04
		//private static string Connection { get { return "Server=server_name;Database=database_name;Uid=username;Pwd=password;"; } }


		private static string Connection { get { return ConfigurationManager.ConnectionStrings["MySQLConnection"].ToString(); } }

		private static string connectionLTE_Ericsson { get { return ConfigurationManager.ConnectionStrings["MySQLConnectionLTE_Ericsson"].ToString(); } }

		public List<LocationDTO> GetLocationData()

		{
			List<LocationDTO> lstLocation = new List<LocationDTO>();

			string query = "SELECT id,object_id,session_id,kpiLTE_UE_GPS FROM lte_drive_test.tt_kpi_data2;";


			// Create an instance of the User class

			LocationDTO location;

			using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, query))
			{
				// Check if the reader returned any rows

				if (reader.HasRows)
				{

					while (reader.Read())
					{
						location = new LocationDTO();

						location.id = Convert.ToInt32(reader[0]);
						location.object_id = Convert.ToInt32(reader[1]);
						location.session_id = Convert.ToInt32(reader[2]);
						//byte[] bytes = (byte[])reader.gets.(3);
						//location.kpiLTE_UE_GPS = reader.GetBytes(3);


						byte[] bfrGPS = (byte[])reader[3];
						byte[] kpiLong = new byte[8];
						byte[] kpiLat = new byte[8];
						byte[] kpiHeight = new byte[8];

						int lat = 0, lng = 0, hgt=0 ;

						for (int i = 0; i < bfrGPS.Length; i++)
						{
							
							if (i < 8)
							{
								kpiLong[lng] = bfrGPS[i];
								lng++;
							}
							else if (i> 7 && i < 16)
							{
								kpiLat[lat] = bfrGPS[i];
								lat++;
							}
							else if (i > 15  && i < 24)
							{
								kpiHeight[hgt] = bfrGPS[i];
								hgt++;
							}
						}

						

						//location.kpiLTE_UE_GPS = bfr;
						//location.strGPSFromB64Str = Convert.ToBase64String(bfr);//BitConverter.ToString(bfr);

						//location.strGPSFromSimpleStr = System.Text.Encoding.Default.GetString(bfr);

						//string strUsingB64 = Convert.ToBase64String(bfr);

						//location.strGPSFromUTF8Str = System.Text.Encoding.UTF8.GetString(bfr, 0, bfr.Length);


						lstLocation.Add(location);

					}

				}

			}


			return lstLocation;

		}



		public List<CQIData> GetCQIData()

		{
			List<CQIData> lstCQIData = new List<CQIData>();

			//string query = "SELECT id,object_id,session_id,kpiLTE_UE_GPS FROM lte_drive_test.tt_kpi_data2;";
			string query = "select LTE_UE_GPS_longitude, LTE_UE_GPS_latitude, LTE_UE_CQI_CQI, period_from, period_to from tblmapplot_new;";


			// Create an instance of the User class

			CQIData CQIData;

			using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, query))
			{
				// Check if the reader returned any rows

				if (reader.HasRows)
				{

					while (reader.Read())
					{
						CQIData = new CQIData();

						CQIData.LTE_UE_GPS_longitude = Convert.ToString(reader[0]);
						CQIData.LTE_UE_GPS_latitude = Convert.ToString(reader[1]);
						CQIData.LTE_UE_CQI_CQI = Convert.ToString(reader[2]);
						
						
						lstCQIData.Add(CQIData);

					}

				}

			}
			return lstCQIData;
		}

		public List<RSRPData> GetRSRPData()
		{
			List<RSRPData> lstRSRPData = new List<RSRPData>();

			string query = "select LTE_UE_GPS_longitude, LTE_UE_GPS_latitude, LTE_UE_RSRP_RSRP, period_from, period_to from tblmapplot_new where LTE_UE_RSRP_RSRP != '';";

			RSRPData RSRPData;

			using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, query))
			{
				// Check if the reader returned any rows

				if (reader.HasRows)
				{

					while (reader.Read())
					{
						RSRPData = new RSRPData();

						RSRPData.LTE_UE_GPS_longitude = Convert.ToString(reader[0]);
						RSRPData.LTE_UE_GPS_latitude = Convert.ToString(reader[1]);
						RSRPData.LTE_UE_RSRP_RSRP = Convert.ToString(reader[2]);


						lstRSRPData.Add(RSRPData);

					}

				}

			}


			return lstRSRPData;

		}



		public List<RSSNRData> GetRSSNRData()
		{
			List<RSSNRData> lstRSSNRData = new List<RSSNRData>();

			string query = "select LTE_UE_GPS_longitude, LTE_UE_GPS_latitude, LTE_UE_RSSNR_RSSNR, period_from, period_to from tblmapplot_new where LTE_UE_RSSNR_RSSNR != '';";

			RSSNRData RSNRData;

			using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, query))
			{
				// Check if the reader returned any rows

				if (reader.HasRows)
				{

					while (reader.Read())
					{
						RSNRData = new RSSNRData();

						RSNRData.LTE_UE_GPS_longitude = Convert.ToString(reader[0]);
						RSNRData.LTE_UE_GPS_latitude = Convert.ToString(reader[1]);
						RSNRData.LTE_UE_RSSNR_RSSNR = Convert.ToString(reader[2]);


						lstRSSNRData.Add(RSNRData);

					}

				}

			}


			return lstRSSNRData;

		}




		public List<CoOrdinateInfo> GetCoOrdinateInfoList()
		{
			List<CoOrdinateInfo> lst = new List<CoOrdinateInfo>();

			return lst;
		}

		// Get folder structure data
		public List<IFSDataModel> GetIFSData()
		{
			List<IFSDataModel> lstIFSData = new List<IFSDataModel>();

			//string query = "select id, parent_id, name from tt_ifs where type = 0;"; // get directories only not files
			//string query = "select id, parent_id, name, type, data_id from tt_ifs;";
			string query = "select ifs.id, ifs.parent_id, ifs.name, ifs.type, ifs.data_id, ifsdata.data " +
							" from tt_ifs ifs " +
							" left join tt_ifs_data ifsdata on ifs.data_id = ifsdata.id;";

			// Create an instance of the User class

			IFSDataModel iFSDataModel;

			//using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connectionLTE_reference, query))
			using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connectionLTE_Ericsson, query))
			{
				// Check if the reader returned any rows

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						iFSDataModel = new IFSDataModel();

						iFSDataModel.id = Convert.ToString(reader["id"]);
						iFSDataModel.parentid = Convert.ToString(reader["parent_id"]);
						iFSDataModel.text = Convert.ToString(reader["name"]);
						iFSDataModel.isDirectory = !Convert.ToBoolean(reader["type"]);
						iFSDataModel.data_id = Convert.ToString(reader["data_id"]);
						if (!(reader["data"] is System.DBNull))
						{
							iFSDataModel.template = System.Text.Encoding.UTF8.GetString((byte[])reader["data"]);
						}
						lstIFSData.Add(iFSDataModel);
					}
				}
			}
			return lstIFSData;
		}

		// Get PLMN Object data
		public List<IFSDataModel> GetPLMNObjectData()
		{
			List<IFSDataModel> lstIFSData = new List<IFSDataModel>();

			string query = "select id, parent_id, object_name from tt_object;";

			// Create an instance of the User class

			IFSDataModel iFSDataModel;

			//using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connectionLTE_reference, query))
			using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connectionLTE_Ericsson, query))
			{
				// Check if the reader returned any rows

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						iFSDataModel = new IFSDataModel();

						iFSDataModel.id = Convert.ToString(reader[0]);
						iFSDataModel.parentid = Convert.ToString(reader[1]);
						iFSDataModel.text = Convert.ToString(reader[2]);

						lstIFSData.Add(iFSDataModel);
					}
				}
			}
			return lstIFSData;
		}

		public string GetReportTemplate()
		{
			string strdata = string.Empty;

			string query = "SELECT id, data FROM tt_ifs_data where id=303;";

			//using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connectionLTE_reference, query))
			using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connectionLTE_Ericsson, query))
			{
				// Check if the reader returned any rows

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						strdata = System.Text.Encoding.UTF8.GetString((byte[])reader["data"]);
					}
				}
			}
			return strdata;
		}

	}
}