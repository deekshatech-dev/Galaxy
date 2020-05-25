using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace NMFReaderForGPS
{
    public static class Program
    {
        // server connection 
        private static string _connection = "datasource = localhost; port = 3306; username = root; password = 1234; Database=galaxy";
        static void Main(string[] args)
        {
            Console.Write("read start");
            // ReadNMLFile();
        }

        public static void ReadNMLFile(string path, int attributeId)
        {
            //	string parentOutPutPath = @"D:\pritesh\GPSPloting\GPSMap\GPSMap\CSVOutPut";
            //	string CSVOutPutFile = "GPSCoordinate";
            string NMFInputFile = path; // @"D:\nmffile\20181025.1.nmf";

            string strdate = "2020-04-28";

            if (path.IndexOf("20181025.1.nmf") > -1)
            {
                strdate = "2018-10-27";
            }
            else if (path.IndexOf("20Apr10_143650.1") > -1)
            {
                strdate = "2018-10-31";
            }

            string IMSI = "";                      // IMSI
            string IMEI = "";                      // IMEI
            string device_name = "";               // Device name
            string device_label = "";              // Device label
                                                   //	string measurement_method = "";        // Measurement method (e.g. drive test)
                                                   //	string measurement_system = "";        // Measurement system (e.g. Nemo Outdoor)
                                                   //	int samples_gps = 0;                // Number of GPS measurement samples
                                                   //	int samples_channel = 0;            // Number of channel measurement samples
                                                   //	int samples_cell = 0;               // Number of cell measurement samples
                                                   //	int lines = 0;                      // Number of lines

            string[] Alllines = File.ReadAllLines(NMFInputFile);


            List<string> sampleGPS = new List<string>();
            List<string> sampleChannel = new List<string>();
            List<string> sampleCELL = new List<string>();

            List<CalculatedGPSInfoDTO> lstFileInfo = new List<CalculatedGPSInfoDTO>();
            CalculatedGPSInfoDTO objFileInfo;

            Random rnd = new Random();
            int iCQI = 0;
            int iRSRP = 0;
            foreach (string line in Alllines)
            {
                objFileInfo = new CalculatedGPSInfoDTO();

                string[] lineParts = line.Split(',');
                string lineHeader = lineParts[0];

                if (lineHeader == "#EI")        // Entry 0 = #EI -> entry 3 = IMEI
                    IMEI = lineParts[3];
                if (lineHeader == "#SI")        // Entry 0 = #SI -> entry 3 = IMSI
                    IMSI = lineParts[3];
                if (lineHeader == "#DN")        // Entry 0 = #DN -> entry 3 = device name
                    device_name = lineParts[3];
                if (lineHeader == "#DL")        // Entry 0 = #DL -> entry 3 = device label
                    device_label = lineParts[3];

                if (lineHeader == "GPS")
                {

                    sampleGPS.Add(line);

                }

                if (lineHeader == "CHI")
                {
                    if (lineParts[3] == "7" || lineParts[3] == "8")
                    {
                        sampleChannel.Add(line);
                        //sample channel
                    }
                }

                if (lineHeader == "CELLMEAS")
                {
                    sampleCELL.Add(line);
                    //sample cell
                }
            }



            int i = 0;                                      // Counter
            int j = 0;
            int k = 0;
            int l = 0;
            string time_stamp = "";                            // Time stamp of any sample as string
            int HH = 0;                                     // Time stamp of any sample as HHMMSS
            int MM = 0;
            double SS = 0;
            string[] time_gps = new string[sampleGPS.Count];          // Time stamp of GPS sample as decimal
            string[] longitude_gps = new string[sampleGPS.Count]; // Coordinates indicated by GPS samples
            string[] latitude_gps = new string[sampleGPS.Count];
            string[] height_gps = new string[sampleGPS.Count];
            string[] time_channel = new string[sampleChannel.Count];  // Time stamp of channel sample as decimal
            string[] GCI = new string[sampleChannel.Count];           // Global cell IDs indicated by channel samples
            string[] line_cell = new string[sampleCELL.Count];



            foreach (string lineItem in Alllines)
            {
                string[] lineParts = lineItem.Split(',');
                string lineHeader = lineParts[0];

                if (lineHeader == "GPS")        // Entry 0 = GPS -> Entry 1 = time stamp, Entry 3/4/5 = longitude/latitude/height above sea level
                {
                    time_stamp = lineParts[1];
                    HH = Convert.ToInt32(time_stamp.Split(':')[0]); //parseFloat(time_stamp.slice(0, 2));
                    MM = Convert.ToInt32(time_stamp.Split(':')[1]);//parseFloat(time_stamp.slice(3, 5));
                    SS = Convert.ToDouble(time_stamp.Split(':')[2]); //parseFloat(time_stamp.slice(6, 12));
                    time_gps[i] = Convert.ToString(Convert.ToDouble(HH + MM / 60 + SS / 3600));
                    longitude_gps[i] = lineParts[3];
                    latitude_gps[i] = lineParts[4];//cCsvReader.asDouble(4);
                    height_gps[i] = lineParts[5];//cCsvReader.asDouble(5);
                    i++;
                }

                if (lineHeader == "CHI")        // Entry 0 = CHI -> Entry 1 = time stamp, Entry 3/4/5 = longitude/latitude/height above sea level
                {
                    if (lineParts[3] == "7" || lineParts[3] == "8")     // Entry 3 = RAT (LTE = 7 or 8
                    {
                        time_stamp = lineParts[1]; ;//cCsvReader.asString(1);				
                        HH = Convert.ToInt32(time_stamp.Split(':')[0]); //parseFloat(time_stamp.slice(0, 2));
                        MM = Convert.ToInt32(time_stamp.Split(':')[1]);//parseFloat(time_stamp.slice(3, 5));
                        SS = Convert.ToDouble(time_stamp.Split(':')[2]); //parseFloat(time_stamp.slice(6, 12));
                        time_channel[j] = Convert.ToString(Convert.ToDouble(HH + MM / 60 + SS / 3600));
                        GCI[j] = Convert.ToString(lineParts[9]); //cCsvReader.asInt(9);
                        j++;
                    }
                }

                if (lineHeader == "CELLMEAS")   // Entry 0 = CELLMEAS -> line indicating cell measurement
                {
                    line_cell[k] = l.ToString();
                    k++;
                }
                l++;
            }

            string readCELLMEAS = "";
            //readCELLMEAS = Alllines[Convert.ToInt32(line_cell[0])];


            string time_kpi = "";                  // Time stamp of KPI sample as decimal
            string time_start = "";                // Start time of 0.5 s interval to which KPI sample is assigned
            string time_end = "";                  // End time of 0.5 s interval to which KPI sample is assigned

            int start_cell = 0;                 // Line indicating current cell measurement
            int end_cell = 0;                   // Line indicating next cell measurement

            double longitude_kpi = 0;            // Coordinates of KPI sample
            double latitude_kpi = 0;
            double height_kpi = 0;


            int RAT = 0;                        // Radio access technology of serving cell (1/2/3/4 = WLAN/GPRS/HSPA/LTE)
            int duplex = 0;                     // Duplex mode of serving cell (0/1 = FDD/TDD)
            int band = 0;                       // ID of serving frequency band
            int carrier = 0;                    // EUARFCN of serving carrier
            double freq = 0;                        // Frequency of serving band
            int cellID = 0;                     // ID of serving cell
            int enodebID = 0;                   // ID of serving eNodeB
            int PCI = 0;                        // PCI of serving cell


            double RSRP = 0;                     // RSRP exact
            double RSRP_round = 0;               // RSRP rounded to 0.5 dB granularity

            double RSRQ = 0;                     // RSRQ exact
            double RSRQ_round = 0;               // RSRQ rounded to 0.5 dB granularity

            double L = 0;                        // Path loss exact
            double L_round = 0;                  // Path loss rounded to 0.5 dB granularity

            double C = 0;                        // RSRP of server absolute
            double I = 0;                        // RSRP summed over all neighbors absolute
            double IC = 0;                       // I / C exact
            double IC_round = 0;                 // I / C rounded to 0.1 granularity

            double RSSNR = 0;                    // RSSNR exact
            double RSSNR_round = 0;              // RSSNR rounded to 0.5 dB granularity
            double nf = 0;                       // UE noise figure

            int cqi = 0;                        // CQI
            int ri = 0;                         // (Maximum) RI
            double utilization = 0;              // PRB utilization

            double tx_pusch = 0;                 // UE power on PUSCH
            double tx_pucch = 0;                 // UE power on PUCCH
            double phr = 0;                      // UE power headroom

            double ta = 0;                       // Timing advance exact
            double ta_round = 0;                 // Timing advance rounded to granularity 1 m
            double attenuation = 0;              // Path loss per km

            double rssi_pusch = 0;               // RSSI on PUSCH exact;
            double rssi_pusch_round = 0;         // RSSI on PUSCH rounded to granularity 0.5 dBm
            double rssi_pucch = 0;               // RSSI on PUCCH
            double psd_pusch = 0;                // Power spectral density on PUSCH

            double BW_DL = 0;                    // Bandwidth occupied by UE on DL exakt
            double BW_DL_round = 0;              // Bandwidth rounded to 1 PRB granularity
            double BW_UL = 0;                    // Bandwidth occupied by UE on UL
            double BW_UL_round = 0;              // Bandwidth rounded to 1 PRB granularity

            double mac_throughput_DL = 0;        // Throughput on MAC layer on DL
            double mac_throughput_UL = 0;        // Throughput on MAC layer on UL
            double mac_BLER = 0;                 // Block error ratio on MAC layer
            double efficiency = 0;               // Spectrum efficiency

            double ppcp_throughput = 0;              // Throughput on PDCP layer
            int pdcp_bearer = 0;                    // Number of parallel PDCP bearers
            double pdcp_throughput = 0;

            string time_attach_last = "";              // Time stamp of last attach request
            string time_attach_current = "";           // Time stamp of current attach request
            bool attach_request = false;             // Status set to true if message comes up, status again set to false after termination of procedure
            bool attach_response = false;            // Status set to true if network responds, status again set to false after new request
            bool attach_complete = true;             // Status set to true if message comes up, status again set to false after new request
            int N_attach = 0;                       // Number of attach requests


            int cellMeasIndexCntr = 0;

            //List<CalculatedGPSInfoDTO> lstFileInfo = new List<CalculatedGPSInfoDTO>();
            //CalculatedGPSInfoDTO objFileInfo;
            int firstCELLMEASLineNumber = Convert.ToInt32(line_cell[0]);

            int xx = 0;

            for (i = 1; i < sampleCELL.Count; i++) // As per discussed with Hussien skip 1st cell measurement so start from 1
            {

                objFileInfo = new CalculatedGPSInfoDTO();

                readCELLMEAS = Alllines[Convert.ToInt32(line_cell[i])]; //cellMeasString;
                string[] lineParts = readCELLMEAS.Split(',');

                time_stamp = lineParts[1];//cCsvReader.asString(1);
                HH = Convert.ToInt32(time_stamp.Split(':')[0]);//parseFloat(time_stamp.slice(0, 2));
                MM = Convert.ToInt32(time_stamp.Split(':')[1]);//parseFloat(time_stamp.slice(3, 5));
                SS = Convert.ToDouble(time_stamp.Split(':')[2]);//parseFloat(time_stamp.slice(6, 12));
                time_kpi = Convert.ToString(Convert.ToDouble(HH + MM / 60 + SS / 3600));
                SS = Math.Round(Convert.ToDouble(2 * SS - Convert.ToDouble(0.5))) / 2;


                string strtime = time_stamp;

                //DateTime dtCombined = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);

                /* sessionTime start and end */
                //
                //
                //
                /* sessionTime start and end */

                double iTempTimeKPI = Convert.ToDouble(time_kpi);

                double longop1 = 0;
                double longop2 = 0;
                double longop3 = 0;
                double longop4 = 0;

                double latop1 = 0;
                double latop2 = 0;
                double latop3 = 0;
                double latop4 = 0;

                int hgtop1 = 0;
                int hgtop2 = 0;
                int hgtop3 = 0;
                int hgtop4 = 0;

                for (j = 0; Convert.ToDouble(time_gps[j]) < iTempTimeKPI && j < sampleGPS.Count - 1; j++)
                {
                    // this loop is for gettting j value
                }

                if (j < 1) // At some points the j value is zero so skip the execution
                    continue;


                objFileInfo.period_from = strdate + " " + strtime;

                objFileInfo.period_to = strdate + " " + strtime;


                longop1 = Convert.ToDouble(longitude_gps[j - 1]);
                longop2 = (iTempTimeKPI - Convert.ToDouble(time_gps[j - 1]));
                longop3 = (Convert.ToDouble(time_gps[j]) - Convert.ToDouble(time_gps[j - 1]));
                longop4 = (Convert.ToDouble(longitude_gps[j]) - Convert.ToDouble(longitude_gps[j - 1]));



                longitude_kpi = longop1 + longop2 / longop3 * longop4;
                latitude_kpi = Convert.ToDouble(latitude_gps[j - 1]) + (iTempTimeKPI - Convert.ToDouble(time_gps[j - 1])) / (Convert.ToDouble(time_gps[j]) - Convert.ToDouble(time_gps[j - 1])) * (Convert.ToDouble(latitude_gps[j]) - Convert.ToDouble(latitude_gps[j - 1]));
                height_kpi = Convert.ToInt32(height_gps[j - 1]) + (iTempTimeKPI - Convert.ToDouble(time_gps[j - 1])) / (Convert.ToDouble(time_gps[j]) - Convert.ToDouble(time_gps[j - 1])) * (Convert.ToInt32(height_gps[j]) - Convert.ToInt32(height_gps[j - 1]));

                objFileInfo.LTE_UE_GPS_Longitude = longitude_kpi.ToString();
                objFileInfo.LTE_UE_GPS_Latitude = latitude_kpi.ToString();
                objFileInfo.LTE_UE_GPS_Height = height_kpi.ToString();

                RAT = Convert.ToInt32(lineParts[3]); //cCsvReader.asInt(3);                                                      // Entry 3 = RAT
                if (RAT == 7 || RAT == 8)                                                       // LTE = 7 or 8 -> set to 4
                    RAT = 4;
                if (RAT == 5)                                                                   // HSPA = 5 -> set to 3
                    RAT = 3;
                if (RAT == 1)                                                                   // GPRS = 1 -> set to 2
                    RAT = 2;
                if (RAT == 20)                                                                  // WLAN = 20 -> set to 1
                    RAT = 1;

                objFileInfo.LTE_UE_SERVER_RAT = RAT.ToString();

                int temp_saleid;

                if (RAT == 4)
                {
                    // cell ID of serving cell
                    for (j = 0; j < sampleChannel.Count && Convert.ToDouble(time_channel[j]) < iTempTimeKPI; j++)
                    {

                    }


                    if (j > 0)
                    {
                        temp_saleid = GCI[j - 1] == "" ? 0 : Convert.ToInt32(GCI[j - 1]);
                        //cellID = Convert.ToInt32(GCI[j - 1]) % 256;                                              // GCI = 20 bits eNodeB ID + 8 bits cell ID
                        cellID = temp_saleid % 256;
                        //enodebID = (Convert.ToInt32(GCI[j - 1]) - cellID) / 256;
                        enodebID = (temp_saleid - cellID) / 256;

                        objFileInfo.LTE_UE_SERVER_enodeb = enodebID.ToString();
                        objFileInfo.LTE_UE_SERVER_cell = cellID.ToString();


                        j = Convert.ToInt32(lineParts[5]); //cCsvReader.asInt(5);                                                    // Entry 5 = number of cells measured by UE
                        I = 0;

                        for (k = 0; k < j; k++)
                        {
                            if (Convert.ToInt32(lineParts[10 * k + 7]) == 0)                                      // Entry 7,17.. = type of cell (0 = server)
                            {
                                // IDs of serving cell

                                band = Convert.ToInt32(lineParts[10 * k + 8]); //cCsvReader.asInt(10 * k + 8);                                    // Entry 8,18.. = frequency band
                                if (band < 80000)                                                   // Frequency band ID < 80000 -> FDD = 0
                                    duplex = 0;
                                else                                                                // Sonst -> TDD = 1
                                    duplex = 1;

                                if (band == 70064)
                                    freq = 430;
                                if (band == 70031 || band == 70072 || band == 70073)
                                    freq = 450;
                                if (band == 70071)
                                    freq = 600;
                                if (band == 70012 || band == 70013 || band == 70014 || band == 70028 || band == 70029 || band == 70067 || band == 70068 || band == 70085)
                                    freq = 700;
                                if (band == 80044)
                                    freq = 750;
                                if (band == 70020 || band == 70027)
                                    freq = 800;
                                if (band == 70005 || band == 70006 || band == 70018 || band == 70019 || band == 70026)
                                    freq = 850;
                                if (band == 70008)
                                    freq = 900;
                                if (band == 70011 || band == 80045 || band == 80050 || band == 80051 || band == 80061 || band == 80087)
                                    freq = 1400;
                                if (band == 70021 || band == 70024 || band == 70032 || band == 70074 || band == 70075 || band == 70076)
                                    freq = 1500;
                                if (band == 70003 || band == 70009 || band == 80035 || band == 80062 || band == 80088)
                                    freq = 1800;
                                if (band == 70002 || band == 70025 || band == 80033 || band == 80036 || band == 80037 || band == 80039)
                                    freq = 1900;
                                if (band == 80034)
                                    freq = 2000;
                                if (band == 70001 || band == 70004 || band == 70010 || band == 70065 || band == 70066 || band == 70070)
                                    freq = 2100;
                                if (band == 70023)
                                    freq = 2200;
                                if (band == 70030 || band == 80040)
                                    freq = 2350;
                                if (band == 70069)
                                    freq = 2500;
                                if (band == 70007 || band == 80038 || band == 80041)
                                    freq = 2600;
                                if (band == 80052)
                                    freq = 3350;
                                if (band == 70022 || band == 80042)
                                    freq = 3500;
                                if (band == 70250 || band == 80048 || band == 80049)
                                    freq = 3625;
                                if (band == 80043)
                                    freq = 3700;
                                if (band == 70252)
                                    freq = 5200;
                                if (band == 70240 || band == 80046)
                                    freq = 5540;
                                if (band == 70255)
                                    freq = 5700;
                                if (band == 80047)
                                    freq = 5900;

                                carrier = Convert.ToInt32(lineParts[10 * k + 9]); //cCsvReader.asInt(10 * k + 9);                                 // Entry 9,19.. = carrier
                                PCI = Convert.ToInt32(lineParts[10 * k + 10]); //cCsvReader.asInt(10 * k + 10);                                    // Entry 10,20.. = PCI

                                objFileInfo.LTE_UE_SERVER_cell = cellID.ToString();


                                objFileInfo.LTE_UE_SERVER_duplex = duplex.ToString();
                                objFileInfo.LTE_UE_SERVER_band = band.ToString();
                                objFileInfo.LTE_UE_SERVER_carrier = carrier.ToString();
                                objFileInfo.LTE_UE_SERVER_frequency = freq.ToString();
                                objFileInfo.LTE_UE_SERVER_PCI = PCI.ToString();


                                // RSRP, RSRQ and L of serving cell

                                RSRP = Convert.ToDouble(lineParts[10 * k + 12]);// cCsvReader.asDouble(10 * k + 12);                                // Entry 12,22.. = RSRP
                                C = Math.Pow(10, Convert.ToDouble(RSRP) / 10); // Math.pow(10, RSRP / 10);
                                RSRP_round = Math.Round(2 * RSRP) / 2;                              // RSRP given with granularity 0.5 dB
                                RSRQ = Convert.ToDouble(lineParts[10 * k + 12]);// cCsvReader.asDouble(10 * k + 13);                                // Entry 13,23.. = RSRP
                                RSRQ_round = Math.Round(2 * RSRQ) / 2;                              // RSRQ given with granularity 0.5 dB
                                L = Convert.ToDouble(lineParts[10 * k + 15]); //cCsvReader.asDouble(10 * k + 15);                                   // Entry 15,25.. = path loss
                                L_round = Math.Round(2 * L) / 2;                                    // Path loss given with granularity 0.5 dB



                                objFileInfo.LTE_UE_RSRP_RSRP = RSRP_round.ToString();
                                objFileInfo.LTE_UE_RSRP_N = "1";
                                objFileInfo.LTE_UE_RSRQ_RSRQ = Convert.ToString(RSRQ_round);
                                objFileInfo.LTE_UE_RSRQ_N = "1";
                                objFileInfo.LTE_UE_L_L = Convert.ToString(L_round);
                                objFileInfo.LTE_UE_L_N = "1";
                                objFileInfo.LTE_UE_L_N = "1";

                            }

                            if (Convert.ToInt32(lineParts[10 * k + 7]) == 1 || Convert.ToInt32(lineParts[10 * k + 7]) == 2)     // Entry 7,17.. = type of cell (1 or 2 = neighbor)
                            {
                                RSRP = Convert.ToDouble(lineParts[10 * k + 12]); //cCsvReader.asDouble(10 * k + 12);                                // Entry 12,22.. = RSRP
                                I += Math.Pow(10, RSRP / 10);
                            }
                        }

                        // I / C of serving cell

                        IC = I / C;
                        IC_round = Math.Round(10 * IC) / 10;                                        // I/C given with granularity 0.1
                        if (IC_round > 10)                                                          // I/C limited to [0,10]
                            IC_round = 10;

                        objFileInfo.LTE_UE_IC_IC = IC_round.ToString();
                        objFileInfo.LTE_UE_IC_N = "1";
                    }


                    // Line domain to next cell measurement

                    start_cell = Convert.ToInt32(line_cell[i]) + 1;
                    if (i < sampleCELL.Count - 1)
                        end_cell = Convert.ToInt32(line_cell[i + 1]);
                    else
                        end_cell = Alllines.Length;


                    // Scan of all lines to next cell measurement

                    for (j = start_cell; j < end_cell; j++)
                    {
                        string nextLineAfterCELLMESR = Alllines[j];
                        string[] innerlineParts = nextLineAfterCELLMESR.Split(',');
                        //cCsvReader.readLine();
                        int thirdValue = 0;
                        int.TryParse(innerlineParts[3], out thirdValue);
                        RAT = thirdValue;  //cCsvReader.asInt(3);                                                  // Entry 3 = RAT

                        // Carrier to interference measurement


                        if (innerlineParts[0] == "CI")                                         // Entry 0 = CI
                        {
                            if ((RAT == 7 || RAT == 8) && Convert.ToInt32(innerlineParts[6]) == 0)                 // LTE = 7 or 8, entry 6 = type of cell (0 = server)
                            {
                                // RSSNR of serving cell

                                RSSNR = Convert.ToDouble(innerlineParts[5]); //cCsvReader.asDouble(5);                                     // Entry 5 = RSSNR
                                RSSNR_round = Math.Round(2 * RSSNR) / 2;                            // RSSNR given with granularity 0.5 dB


                                objFileInfo.LTE_UE_RSSNR_RSSNR = RSSNR_round.ToString();
                                objFileInfo.LTE_UE_RSSNR_N = "1";

                                // DL utilization of serving cell

                                ri = Convert.ToInt32(innerlineParts[7]); //cCsvReader.asDouble(7);                                        // Entry 7 = maximum RI (number of Tx antennas)
                                                                         // Utiliaztion given with granularity 1 %
                                utilization = Math.Round(100 * (Math.Pow(10, (RSRP - RSRQ) / 10) - Math.Pow(10, -12.6)) / (12 * ri * (1 + IC) * Math.Pow(10, RSRP / 10)));
                                if (utilization < 0)                                                // Utilization limited to [0..100]
                                    utilization = 0;
                                if (utilization > 100)
                                    if (utilization > 100)
                                        utilization = 100;

                                objFileInfo.LTE_UE_UTILIZATION_DL_utilization = utilization.ToString();
                                objFileInfo.LTE_UE_UTILIZATION_DL_N = "1";

                                // UE receiver noise (within isolated cells with RSRP < -115 dBm only)

                                if (RSRP < -115 && IC < 0.1)
                                {
                                    nf = Math.Round(2 * (RSRP - RSSNR - 132.2)) / 2;                // Noise figure given with granularity 0.5 dB
                                    if (nf < 0)                                                     // Noise figure limited to [0,infinity] dB
                                        nf = 0;

                                    objFileInfo.LTE_UE_UTILIZATION_DL_utilization = utilization.ToString();
                                    objFileInfo.LTE_UE_UTILIZATION_DL_N = "1";
                                }
                            }
                        }


                        // UE power control measurement

                        if (innerlineParts[0] == "TXPC")                                       // Entry 0 = TXPC
                        {
                            if (RAT == 7 || RAT == 8)                                               // LTE = 7 or 8
                            {
                                // UE power on PUSCH and PUCCH and UE power headroom for serving cell

                                tx_pusch = Convert.ToDouble(innerlineParts[4] == "" ? "0" : innerlineParts[4]); //cCsvReader.asDouble(4);                                  // Entry 4 = UE power on PUSCH
                                tx_pusch = Math.Round(2 * tx_pusch) / 2;                            // UE power given with granularity 0.5 dB
                                tx_pucch = Convert.ToDouble(innerlineParts[5] == "" ? "0" : innerlineParts[5]); //cCsvReader.asDouble(5);                                  // Entry 5 = UE power on PUSCH
                                tx_pucch = Math.Round(2 * tx_pucch) / 2;                            // UE power given with granularity 0.5 dB

                                objFileInfo.LTE_UE_TX_POWER_PUSCH_TXpower = tx_pusch.ToString();
                                objFileInfo.LTE_UE_TX_POWER_PUSCH_N = "1";
                                objFileInfo.LTE_UE_TX_POWER_PUCCH_TXpower = tx_pucch.ToString();
                                objFileInfo.LTE_UE_TX_POWER_PUCCH_N = "1";

                                if (innerlineParts[6] != "")                                   // Entry 6 = UE power headroom not always indicated
                                {
                                    phr = Convert.ToDouble(innerlineParts[6]); //cCsvReader.asDouble(6);
                                    phr = Math.Round(2 * phr) / 2;                                  // UE power headroom given with granularity 0.5 dB

                                    objFileInfo.LTE_UE_PHR_PHR = phr.ToString();
                                    objFileInfo.LTE_UE_PHR_N = "1";
                                }

                                // RSSI on PUSCH and PUCCH for serving cell

                                rssi_pusch = tx_pusch - L;
                                rssi_pusch_round = Math.Round(2 * rssi_pusch) / 2;                  // RSSI given with granularity 0.5 dB
                                rssi_pucch = Math.Round(2 * (tx_pucch - L)) / 2;                    // RSSI given with granularity 0.5 dB

                                objFileInfo.LTE_UE_RSSI_PUSCH_RSSI = rssi_pusch_round.ToString();
                                objFileInfo.LTE_UE_RSSI_PUSCH_N = "1";
                                objFileInfo.LTE_UE_RSSI_PUCCH_RSSI = rssi_pucch.ToString();
                                objFileInfo.LTE_UE_RSSI_PUCCH_N = "1";
                            }
                        }


                        // Timing advance measurement

                        if (innerlineParts[0] == "TAD")                                        // Entry 0 = TAD
                        {
                            if ((RAT == 7 || RAT == 8) && Convert.ToInt32(innerlineParts[5]) == 0)                 // LTE = 7 or 8, entry 5 = type of cell (0 = server)
                            {
                                // Timing advance

                                ta = 78.12 * Convert.ToDouble(innerlineParts[4]); //cCsvReader.asDouble(4);                                // Entry 4 = TA
                                ta_round = Math.Round(ta);                                          // TA given with granularity 1 m

                                objFileInfo.LTE_UE_TA_TA = ta.ToString();
                                objFileInfo.LTE_UE_TA_N = "1";

                                // Path loss per km

                                attenuation = Math.Round(2000 * (L_round - 60) / ta) / 2;           // Path loss per km given with granularity 0.5 dB / km, 60 dB loss on first 0.1 lambda

                                objFileInfo.LTE_UE_ATTENUATION_attenuation = attenuation.ToString();
                                objFileInfo.LTE_UE_ATTENUATION_N = "1";
                            }
                        }

                        // Link adaptation measurement DL
                        int LA = 0;
                        int PRB = 0;
                        double weight = 0;
                        int N = 0;
                        if (innerlineParts[0] == "PLAID")                                      // Entry 0 = PLAID
                        {
                            if ((RAT == 7 || RAT == 8) && Convert.ToInt32(innerlineParts[9]) == 0)                 // LTE = 7 or 8, entry 9 = type of cell (0 = server)
                            {
                                // Bandwidth occupied by UE in serving cell

                                LA = Convert.ToInt32(innerlineParts[10]); //cCsvReader.asInt(10);                                      // Entry 10 = number of link adaptation sets LA (each set again has 9 entries)
                                PRB = Convert.ToInt32(innerlineParts[12 + 9 * LA]); //cCsvReader.asInt(12 + 9 * LA);                                // Entry 12 + 9 LA = number of PRB sets PRB (each set again has 3 entries)
                                BW_DL = 0;
                                weight = 0;
                                for (k = 0; k < PRB; k++)                                           // Entry 14 + 9 LA + 3 PRB = percentage specific bandwidth per TTI is coming up
                                {                                                                   // Entry 15 + 9 LA + 3 PRB = specific bandwidth per TTI
                                    if (innerlineParts[14 + 9 * LA + 3 * k] != "" && innerlineParts[15 + 9 * LA + 3 * k] != "")
                                    {
                                        N = Convert.ToInt32(innerlineParts[15 + 9 * LA + 3 * k]); //cCsvReader.asInt(15 + 9 * LA + 3 * k);
                                        if (N > 0)
                                        {
                                            BW_DL += N * Convert.ToDouble(innerlineParts[14 + 9 * LA + 3 * k]); //cCsvReader.asDouble(14 + 9 * LA + 3 * k);
                                            weight += Convert.ToDouble(innerlineParts[14 + 9 * LA + 3 * k]);//cCsvReader.asDouble(14 + 9 * LA + 3 * k);
                                        }
                                    }
                                }
                                if (weight > 0)
                                {
                                    BW_DL /= weight;
                                    BW_DL_round = Math.Round(BW_DL);                                // Average bandwidth given with granularity 1 PRB

                                    objFileInfo.LTE_UE_MAC_BW_DL_PRBs = BW_DL_round.ToString();
                                    objFileInfo.LTE_UE_MAC_BW_DL_N = "1";
                                }
                            }
                        }


                        // CQI measurement

                        if (innerlineParts[0] == "CQI")                                        // Entry 0 = CQI
                        {
                            if ((RAT == 7 || RAT == 8) &&
                                Convert.ToInt32(innerlineParts[5]) > 0 && Convert.ToInt32(innerlineParts[12]) == 0)               // LTE = 7 or 8, entry 5 = duration of CQI sampling, entry 12 = type of cell (0 = server)
                            {
                                // CQI of serving cell

                                cqi = Convert.ToInt32(innerlineParts[7] == "" ? "0" : innerlineParts[7]); //cCsvReader.asInt(7);                                          // Entry 7 = CQI

                                objFileInfo.LTE_UE_CQI_CQI = cqi.ToString();
                                objFileInfo.LTE_UE_CQI_N = innerlineParts[5]; //cCsvReader.asInt(5));

                                // RI of serving cell

                                ri = Convert.ToInt32(innerlineParts[13]); //cCsvReader.asInt(13);                                          // Entry 13 = maximum RI
                                for (k = 1; k <= ri; k++)
                                {
                                    objFileInfo.LTE_UE_RI_RI = k.ToString();                      // Entry 15,17... = percentage of samples per RI
                                    objFileInfo.LTE_UE_RI_N = Convert.ToString(Math.Round(Convert.ToDouble(innerlineParts[13 + 2 * k])));
                                }
                            }
                        }

                        // Link adaptation measurement UL

                        if (innerlineParts[0] == "PLAIU")                                      // Entry 0 = PLAIU
                        {
                            if ((RAT == 7 || RAT == 8) && Convert.ToInt32(innerlineParts[9]) == 0)                 // LTE = 7 or 8, entry 9 = type of cell (0 = server)
                            {
                                // Bandwidth occupied by UE in serving cell

                                LA = Convert.ToInt32(innerlineParts[10]);//cCsvReader.asInt(10);                                          // Entry 10 = number of link adaptation sets LA (each set again has 9 entries)
                                PRB = Convert.ToInt32(innerlineParts[12 + 9 * LA]);//cCsvReader.asInt(12 + 9 * LA);                                    // Entry 12 + 9 LA = number of PRB sets PRB (each set again has 3 entries)
                                BW_UL = 0;
                                weight = 0;
                                for (k = 0; k < PRB; k++)                                           // Entry 14 + 9 LA + 3 PRB = percentage specific bandwidth per TTI is coming up
                                {                                                                   // Entry 15 + 9 LA + 3 PRB = specific bandwidth per TTI
                                    if (innerlineParts[14 + 9 * LA + 3 * k] != "" && innerlineParts[15 + 9 * LA + 3 * k] != "")
                                    {
                                        N = Convert.ToInt32(innerlineParts[15 + 9 * LA + 3 * k]);
                                        if (N > 0)
                                        {
                                            BW_UL += N * Convert.ToDouble(innerlineParts[14 + 9 * LA + 3 * k]); //cCsvReader.asDouble(14 + 9 * LA + 3 * k);
                                            weight += Convert.ToDouble(innerlineParts[14 + 9 * LA + 3 * k]); //cCsvReader.asDouble(14 + 9 * LA + 3 * k);
                                        }
                                    }
                                }
                                if (weight > 0)
                                {
                                    BW_UL /= weight;
                                    BW_UL_round = Math.Round(BW_UL);                                // Average bandwidth given with granularity 1 PRB

                                    objFileInfo.LTE_UE_MAC_BW_UL_PRBs = BW_UL_round.ToString();
                                    objFileInfo.LTE_UE_MAC_BW_UL_N = "1";
                                }

                                // Power spectral density on PUSCH for serving cell

                                if (BW_UL > 1)
                                {
                                    psd_pusch = Math.Round(2 * (rssi_pucch - 4.343 * Math.Log(BW_UL))) / 2; // Power spectral density given with granularity 1 dBm/PRB

                                    objFileInfo.LTE_UE_PSD_PUSCH_PSD = psd_pusch.ToString();
                                    objFileInfo.LTE_UE_PSD_PUSCH_N = "1";
                                }
                            }
                        }


                        // MAC measurement DL

                        if (innerlineParts[0] == "MACRATE")                                    // Entry 0 = MACRATE
                        {
                            if ((RAT == 7 || RAT == 8) && Convert.ToInt32(innerlineParts[13]) == 0)                // LTE = 7 or 8, entry 13 = type of cell (0 = server)
                            {
                                // MAC throughput and BLER for serving cell

                                mac_BLER = Math.Round(Convert.ToDouble(innerlineParts[6]));                      // Entry 6 = BLER, given with granularity 1 %
                                mac_throughput_DL = Math.Round(Convert.ToDouble(innerlineParts[4]) / 1000000);   // Entry 4 = throughput, given with granularity 1 Mbit/s

                                objFileInfo.LTE_UE_MAC_BLER_DL_BLER = mac_BLER.ToString();
                                objFileInfo.LTE_UE_MAC_BLER_DL_N = "1";
                                objFileInfo.LTE_UE_MAC_THROUGHPUT_DL_throughput = mac_throughput_DL.ToString();
                                objFileInfo.LTE_UE_MAC_THROUGHPUT_DL_N = "1";

                                // Spectrum efficiency for serving cell

                                if (BW_DL > 1)
                                {
                                    efficiency = Math.Round(10 * mac_throughput_DL / (0.18 * BW_DL)) / 10;  // Spectrum efficiency given with granularity 0.1 Bit/s/Hz

                                    objFileInfo.LTE_UE_MAC_EFFICIENCY_DL_efficiency = efficiency.ToString();
                                    objFileInfo.LTE_UE_MAC_EFFICIENCY_DL_N = "1";
                                }
                            }
                        }


                        // MAC measurement UL

                        if (innerlineParts[0] == "MACRATEU")                                   // Entry 0 = MACRATEU
                        {
                            if ((RAT == 7 || RAT == 8) && Convert.ToInt32(innerlineParts[12]) == 0)                // LTE = 7 or 8, entry 12 = type of cell (0 = server)
                            {
                                // MAC throughput and BLER for serving cell

                                mac_BLER = Math.Round(Convert.ToDouble(innerlineParts[6]));                      // Entry 6 = BLER, given with granularity 1 %

                                mac_throughput_UL = Math.Round(Convert.ToDouble(innerlineParts[4]) / 100000) / 10;   // Entry 4 = throughput, given with granularity 0.1 Mbit/s

                                objFileInfo.LTE_UE_MAC_BLER_UL_BLER = mac_BLER.ToString();
                                objFileInfo.LTE_UE_MAC_BLER_UL_N = "1";
                                objFileInfo.LTE_UE_MAC_THROUGHPUT_UL_throughput = mac_throughput_UL.ToString();
                                objFileInfo.LTE_UE_MAC_THROUGHPUT_UL_N = "1";

                                // Spectrum efficiency for serving cell

                                if (BW_UL > 1)
                                {
                                    efficiency = Math.Round(10 * mac_throughput_UL / (0.18 * BW_UL)) / 10;  // Spectrum efficiency given with granularity 0.1 Bit/s/Hz

                                    objFileInfo.LTE_UE_MAC_EFFICIENCY_UL_efficiency = efficiency.ToString();
                                    objFileInfo.LTE_UE_MAC_EFFICIENCY_UL_N = "1";
                                }
                            }
                        }

                        // PDCP measurement DL

                        if (innerlineParts[0] == "PDCPRATED")                                  // Entry 0 = PDCPRATED
                        {
                            if (RAT == 7 || RAT == 8)                                               // LTE = 7 or 8
                            {
                                // PDCP throughput and number of parallel bearers

                                pdcp_throughput = Math.Round(Convert.ToDouble(innerlineParts[5]) / 1000000); // Entry 5 = throughput, given with granularity 1 Mbit/s
                                pdcp_bearer = Convert.ToInt32(innerlineParts[7]); //cCsvReader.asInt(7);                                  // Entry 7 = number of parallel bearers

                                objFileInfo.LTE_UE_PDCP_THROUGHPUT_DL_throughput = pdcp_throughput.ToString();
                                objFileInfo.LTE_UE_PDCP_THROUGHPUT_DL_N = "1";
                                objFileInfo.LTE_UE_PDCP_BEARER_DL_bearer = pdcp_bearer.ToString();
                                objFileInfo.LTE_UE_PDCP_BEARER_DL_N = "1";
                            }
                        }

                        // PDCP measurement UL

                        if (innerlineParts[0] == "PDCPRATEU")                                  // Entry 0 = PDCPRATEU
                        {
                            if (RAT == 7 || RAT == 8)                                               // LTE = 7 or 8
                            {
                                // PDCP throughput and number of parallel bearers

                                pdcp_throughput = Math.Round(Convert.ToDouble(innerlineParts[5]) / 100000) / 10;// Entry 5 = throughput, given with granularity 0.1 Mbit/s
                                pdcp_bearer = Convert.ToInt32(innerlineParts[7]);//cCsvReader.asInt(7);                                  // Entry 7 = number of parallel bearers

                                objFileInfo.LTE_UE_PDCP_THROUGHPUT_UL_throughput = pdcp_throughput.ToString();
                                objFileInfo.LTE_UE_PDCP_THROUGHPUT_UL_N = "1";
                                objFileInfo.LTE_UE_PDCP_BEARER_UL_bearer = pdcp_bearer.ToString();
                                objFileInfo.LTE_UE_PDCP_BEARER_UL_N = "1";
                            }
                        }


                    }

                }

                cellMeasIndexCntr++;

                lstFileInfo.Add(objFileInfo);
                xx++;
            }

            if (lstFileInfo != null && lstFileInfo.Count > 0)
            {
                //Save this list into Database
                savetodb(lstFileInfo, attributeId);
            }


        }

        public static void SaveClassTable()
        {
            string Command = "INSERT INTO tt_class(id,gid,parent_id,name,description,displayname,objectdisplay);";
            long plmnId = 0;


            using (var mConnection = new MySqlConnection(_connection))
            {
                mConnection.Open();
                MySqlTransaction transaction = mConnection.BeginTransaction();

                var ds = new DataSet();

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new[]
                    {

                        new DataColumn("id", typeof(int)),
                        new DataColumn("gid", typeof(string)),
                        new DataColumn("parent_id", typeof(int)),
                        new DataColumn("name", typeof(string)),
                        new DataColumn("description", typeof(string)),
                        new DataColumn("displayname", typeof(string)),
                        new DataColumn("objectdisplay", typeof(string))
                });

                ds.Tables.Add(dt);

                var mySqlDataAdapter = new MySqlDataAdapter();

                mySqlDataAdapter.InsertCommand = new MySqlCommand(Command, mConnection);
                mySqlDataAdapter.InsertCommand.Parameters.Add("@id", MySqlDbType.Int64, 1500, "id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@gid", MySqlDbType.VarChar, 1500, "gid");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@parent_id", MySqlDbType.Int64, 1500, "parent_id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@name", MySqlDbType.VarChar, 1500, "name");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@description", MySqlDbType.VarChar, 1500, "description");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@displayname", MySqlDbType.VarChar, 1500, "displayname");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@objectdisplay", MySqlDbType.VarChar, 1500, "objectdisplay");
                mySqlDataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                DataRow plmnRow = ds.Tables[0].NewRow();
                plmnRow["gid"] = new Guid().ToString();
                plmnRow["parent_id"] = "0";
                plmnRow["name"] = "PLMN";
                plmnRow["description"] = "PLMN";
                plmnRow["displayname"] = "PLMN";
                ds.Tables[0].Rows.Add(plmnRow);

                transaction.Commit();

                plmnId = mySqlDataAdapter.InsertCommand.LastInsertedId;
            }

            using (var mConnection = new MySqlConnection(_connection))
            {
                mConnection.Open();
                MySqlTransaction transaction = mConnection.BeginTransaction();

                var ds = new DataSet();

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new[]
                {
                    new DataColumn("id", typeof(int)),
                    new DataColumn("gid", typeof(string)),
                    new DataColumn("parent_id", typeof(int)),
                    new DataColumn("name", typeof(string)),
                    new DataColumn("description", typeof(string)),
                    new DataColumn("displayname", typeof(string)),
                    new DataColumn("objectdisplay", typeof(string))
                });

                ds.Tables.Add(dt);

                var mySqlDataAdapter = new MySqlDataAdapter();

                mySqlDataAdapter.InsertCommand = new MySqlCommand(Command, mConnection);
                mySqlDataAdapter.InsertCommand.Parameters.Add("@id", MySqlDbType.Int64, 1500, "id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@gid", MySqlDbType.VarChar, 1500, "gid");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@parent_id", MySqlDbType.Int64, 1500, "parent_id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@name", MySqlDbType.VarChar, 1500, "name");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@description", MySqlDbType.VarChar, 1500, "description");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@displayname", MySqlDbType.VarChar, 1500, "displayname");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@objectdisplay", MySqlDbType.VarChar, 1500, "objectdisplay");
                mySqlDataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                DataRow userRow = ds.Tables[0].NewRow();
                userRow["gid"] = new Guid().ToString();
                userRow["parent_id"] = plmnId;
                userRow["name"] = "USER";
                userRow["description"] = "User";
                userRow["displayname"] = "User";
                ds.Tables[0].Rows.Add(userRow);

                mySqlDataAdapter.UpdateBatchSize = 100;
                mySqlDataAdapter.Update(ds, "table1");

                transaction.Commit();
            }
        }

        public static void SaveAttributeValue(string imei, string imsi, string ue_label, string ue_name)
        {
            string Command = "INSERT INTO tt_attribute_value(attributeid,class_id,name,imei,imsi,ue_label,ue_name,location);";
            long plmnId = 0;


            using (var mConnection = new MySqlConnection(_connection))
            {
                mConnection.Open();
                MySqlTransaction transaction = mConnection.BeginTransaction();

                var ds = new DataSet();

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new[]
                    {

                        new DataColumn("attributeid", typeof(int)),
                        new DataColumn("class_id", typeof(string)),
                       // new DataColumn("tm_name", typeof(string)),
                        new DataColumn("name", typeof(string)),
                        new DataColumn("imei", typeof(string)),
                        new DataColumn("imsi", typeof(string)),
                        new DataColumn("ue_label", typeof(string)),
                        new DataColumn("ue_name", typeof(string)),
                        new DataColumn("location", typeof(string))
                       // new DataColumn("ue_location", typeof(string))
                });

                ds.Tables.Add(dt);

                var mySqlDataAdapter = new MySqlDataAdapter();

                mySqlDataAdapter.InsertCommand = new MySqlCommand(Command, mConnection);
                mySqlDataAdapter.InsertCommand.Parameters.Add("@attributeid", MySqlDbType.Int64, 11, "attributeid");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@class_id", MySqlDbType.VarChar, 45, "class_id");
                // mySqlDataAdapter.InsertCommand.Parameters.Add("@tm_name", MySqlDbType.VarChar, 45, "tm_name");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@name", MySqlDbType.VarChar, 45, "name");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@imei", MySqlDbType.VarChar, 45, "imei");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@imsi", MySqlDbType.VarChar, 45, "imsi");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@ue_label", MySqlDbType.VarChar, 45, "ue_label");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@ue_name", MySqlDbType.VarChar, 45, "ue_name");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@location", MySqlDbType.VarChar, 45, "location");
                // mySqlDataAdapter.InsertCommand.Parameters.Add("@ue_location", MySqlDbType.VarChar, 45, "ue_location");
                mySqlDataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                DataRow plmnRow = ds.Tables[0].NewRow();
                plmnRow["class_id"] = "1";
                plmnRow["name"] = "Testing value";
                // plmnRow["tm_name"] = "Testing value";
                plmnRow["imei"] = "Test Imei";
                plmnRow["imsi"] = "Test Imsi";
                plmnRow["ue_label"] = "Test Label";
                plmnRow["ue_name"] = "Test Name";
                // plmnRow["ue_location"] = "Test Location";
                plmnRow["location"] = "Test Location";
                ds.Tables[0].Rows.Add(plmnRow);

                mySqlDataAdapter.UpdateBatchSize = 100;
                mySqlDataAdapter.Update(ds, "table1");
                transaction.Commit();

                plmnId = mySqlDataAdapter.InsertCommand.LastInsertedId;
            }
        }
        public static void savetodb(List<CalculatedGPSInfoDTO> lstFileInfo, int attributeId)
        {

            string Command = "INSERT INTO tt_kpidata(id,object_id,session_id,attribute_id,period_from,period_to,LTE_UE_GPS_longitude,LTE_UE_GPS_latitude,LTE_UE_GPS_height,LTE_UE_SERVER_RAT,LTE_UE_SERVER_DUPLEX,LTE_UE_SERVER_BAND,LTE_UE_SERVER_CARRIER,LTE_UE_SERVER_FREQUENCY,LTE_UE_SERVER_ENODEB,LTE_UE_SERVER_CELL,LTE_UE_SERVER_PCI,LTE_UE_RSRP_RSRP,LTE_UE_RSRP_N,LTE_UE_RSRQ_RSRQ,LTE_UE_RSRQ_N,LTE_UE_L_L,LTE_UE_L_N,LTE_UE_IC_IC,LTE_UE_IC_N,LTE_UE_RSSNR_RSSNR,LTE_UE_RSSNR_N,LTE_UE_NF_UE_NF,LTE_UE_NF_UE_NF_N,LTE_UE_CQI_CQI,LTE_UE_CQI_N,LTE_UE_RI_RI,LTE_UE_RI_N,LTE_UE_UTILIZATION_DL_UTILIZAYION,LTE_UE_UTILIZATION_DL_N,LTE_UE_TX_POWER_PUSCH_TXPOWER,LTE_UE_TX_POWER_PUSCH_N,LTE_UE_TX_POWER_PUCCH_TXPOWER,LTE_UE_TX_POWER_PUCCH_N,LTE_UE_PHR_PHR,LTE_UE_PHR_N,LTE_UE_TA_TA,LTE_UE_TA_N,LTE_UE_ATTENUATION_ATTENUATION,LTE_UE_ATTENUATION_N,LTE_UE_RSSI_PUSCH_RSSI,LTE_UE_RSSI_PUSCH_N,LTE_UE_RSSI_PUCCH_RSSI,LTE_UE_RSSI_PUCCH_N,LTE_UE_PSD_PUSCH_PSD,LTE_UE_PSD_PUSCH_N,LTE_UE_MAC_BW_DL_PRBs,LTE_UE_MAC_BW_DL_N,LTE_UE_MAC_BW_UL_PRBs,LTE_UE_MAC_BW_UL_N,LTE_UE_MAC_THROUGHPUT_DL_throughput,LTE_UE_MAC_THROUGHPUT_DL_N,LTE_UE_MAC_THROUGHPUT_UL_throughput,LTE_UE_MAC_THROUGHPUT_UL_N,LTE_UE_MAC_BLER_DL_BLER,LTE_UE_MAC_BLER_DL_N,LTE_UE_MAC_BLER_UL_BLER,LTE_UE_MAC_BLER_UL_N,LTE_UE_MAC_EFFICIENCY_DL_EFFICIENCY,LTE_UE_MAC_EFFICIENCY_DL_N,LTE_UE_MAC_EFFICIENCY_UL_EFFICIENCY,LTE_UE_MAC_EFFICIENCY_UL_N,LTE_UE_PDCP_THROUGHPUT_DL_THROUGHPUT,LTE_UE_PDCP_THROUGHPUT_DL_N,LTE_UE_PDCP_THROUGHPUT_UL_THROUGHPUT,LTE_UE_PDCP_THROUGHPUT_UL_N,LTE_UE_PDCP_BEARER_DL_BEARER,LTE_UE_PDCP_BEARER_DL_N,LTE_UE_PDCP_BEARER_UL_BEARER,LTE_UE_PDCP_BEARER_UL_N,LTE_UE_ATTACH_SUCCESS_N_ATTEMPT,LTE_UE_ATTACH_SUCCESS_N_SUCCESS,LTE_UE_ATTACH_FAILURE_N_ATTEMPT,LTE_UE_ATTACH_FAILURE_N_FAILURE_noresponse,LTE_UE_ATTACH_FAILURE_N_FAILURE_REJECT,LTE_UE_ATTACH_FAILURE_N_FAILURE_UE) VALUES (@id,@object_id,@session_id,@attribute_id,@period_from,@period_to,@LTE_UE_GPS_longitude,@LTE_UE_GPS_latitude,@LTE_UE_GPS_height,@LTE_UE_SERVER_RAT,@LTE_UE_SERVER_DUPLEX,@LTE_UE_SERVER_BAND,@LTE_UE_SERVER_CARRIER,@LTE_UE_SERVER_FREQUENCY,@LTE_UE_SERVER_ENODEB,@LTE_UE_SERVER_CELL,@LTE_UE_SERVER_PCI,@LTE_UE_RSRP_RSRP,@LTE_UE_RSRP_N,@LTE_UE_RSRQ_RSRQ,@LTE_UE_RSRQ_N,@LTE_UE_L_L,@LTE_UE_L_N,@LTE_UE_IC_IC,@LTE_UE_IC_N,@LTE_UE_RSSNR_RSSNR,@LTE_UE_RSSNR_N,@LTE_UE_NF_UE_NF,@LTE_UE_NF_UE_NF_N,@LTE_UE_CQI_CQI,@LTE_UE_CQI_N,@LTE_UE_RI_RI,@LTE_UE_RI_N,@LTE_UE_UTILIZATION_DL_UTILIZAYION,@LTE_UE_UTILIZATION_DL_N,@LTE_UE_TX_POWER_PUSCH_TXPOWER,@LTE_UE_TX_POWER_PUSCH_N,@LTE_UE_TX_POWER_PUCCH_TXPOWER,@LTE_UE_TX_POWER_PUCCH_N,@LTE_UE_PHR_PHR,@LTE_UE_PHR_N,@LTE_UE_TA_TA,@LTE_UE_TA_N,@LTE_UE_ATTENUATION_ATTENUATION,@LTE_UE_ATTENUATION_N,@LTE_UE_RSSI_PUSCH_RSSI,@LTE_UE_RSSI_PUSCH_N,@LTE_UE_RSSI_PUCCH_RSSI,@LTE_UE_RSSI_PUCCH_N,@LTE_UE_PSD_PUSCH_PSD,@LTE_UE_PSD_PUSCH_N,@LTE_UE_MAC_BW_DL_PRBs,@LTE_UE_MAC_BW_DL_N,@LTE_UE_MAC_BW_UL_PRBs,@LTE_UE_MAC_BW_UL_N,@LTE_UE_MAC_THROUGHPUT_DL_throughput,@LTE_UE_MAC_THROUGHPUT_DL_N,@LTE_UE_MAC_THROUGHPUT_UL_throughput,@LTE_UE_MAC_THROUGHPUT_UL_N,@LTE_UE_MAC_BLER_DL_BLER,@LTE_UE_MAC_BLER_DL_N,@LTE_UE_MAC_BLER_UL_BLER,@LTE_UE_MAC_BLER_UL_N,@LTE_UE_MAC_EFFICIENCY_DL_EFFICIENCY,@LTE_UE_MAC_EFFICIENCY_DL_N,@LTE_UE_MAC_EFFICIENCY_UL_EFFICIENCY,@LTE_UE_MAC_EFFICIENCY_UL_N,@LTE_UE_PDCP_THROUGHPUT_DL_THROUGHPUT,@LTE_UE_PDCP_THROUGHPUT_DL_N,@LTE_UE_PDCP_THROUGHPUT_UL_THROUGHPUT,@LTE_UE_PDCP_THROUGHPUT_UL_N,@LTE_UE_PDCP_BEARER_DL_BEARER,@LTE_UE_PDCP_BEARER_DL_N,@LTE_UE_PDCP_BEARER_UL_BEARER,@LTE_UE_PDCP_BEARER_UL_N,@LTE_UE_ATTACH_SUCCESS_N_ATTEMPT,@LTE_UE_ATTACH_SUCCESS_N_SUCCESS,@LTE_UE_ATTACH_FAILURE_N_ATTEMPT,@LTE_UE_ATTACH_FAILURE_N_FAILURE_noresponse,@LTE_UE_ATTACH_FAILURE_N_FAILURE_REJECT,@LTE_UE_ATTACH_FAILURE_N_FAILURE_UE);";
            using (var mConnection = new MySqlConnection(_connection))
            {
                mConnection.Open();
                MySqlTransaction transaction = mConnection.BeginTransaction();

                //Obtain a dataset, obviously a "select *" is not the best way...
                //var mySqlDataAdapterSelect = new MySqlDataAdapter("select * from 5gdetail", mConnection);

                var ds = new DataSet();

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new[]
                    {

                        new DataColumn("id", typeof(int)),
                        new DataColumn("object_id", typeof(int)),
                        new DataColumn("session_id", typeof(int)),
                        new DataColumn("attribute_id", typeof(int)),
                        new DataColumn("period_from", typeof(DateTime)),
                        new DataColumn("period_to", typeof(DateTime)),
                        new DataColumn("LTE_UE_GPS_longitude", typeof(string)),
                        new DataColumn("LTE_UE_GPS_latitude", typeof(string)),
                        new DataColumn("LTE_UE_GPS_height", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_RAT", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_DUPLEX", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_BAND", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_CARRIER", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_FREQUENCY", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_ENODEB", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_CELL", typeof(string)),
                        new DataColumn("LTE_UE_SERVER_PCI", typeof(string)),
                        new DataColumn("LTE_UE_RSRP_RSRP", typeof(string)),
                        new DataColumn("LTE_UE_RSRP_N", typeof(string)),
                        new DataColumn("LTE_UE_RSRQ_RSRQ", typeof(string)),
                        new DataColumn("LTE_UE_RSRQ_N", typeof(string)),
                        new DataColumn("LTE_UE_L_L", typeof(string)),
                        new DataColumn("LTE_UE_L_N", typeof(string)),
                        new DataColumn("LTE_UE_IC_IC", typeof(string)),
                        new DataColumn("LTE_UE_IC_N", typeof(string)),
                        new DataColumn("LTE_UE_RSSNR_RSSNR", typeof(string)),
                        new DataColumn("LTE_UE_RSSNR_N", typeof(string)),
                        new DataColumn("LTE_UE_NF_UE_NF", typeof(string)),
                        new DataColumn("LTE_UE_NF_UE_NF_N", typeof(string)),
                        new DataColumn("LTE_UE_CQI_CQI", typeof(string)),
                        new DataColumn("LTE_UE_CQI_N", typeof(string)),
                        new DataColumn("LTE_UE_RI_RI", typeof(string)),
                        new DataColumn("LTE_UE_RI_N", typeof(string)),
                        new DataColumn("LTE_UE_UTILIZATION_DL_UTILIZAYION", typeof(string)),
                        new DataColumn("LTE_UE_UTILIZATION_DL_N", typeof(string)),
                        new DataColumn("LTE_UE_TX_POWER_PUSCH_TXPOWER", typeof(string)),
                        new DataColumn("LTE_UE_TX_POWER_PUSCH_N", typeof(string)),
                        new DataColumn("LTE_UE_TX_POWER_PUCCH_TXPOWER", typeof(string)),
                        new DataColumn("LTE_UE_TX_POWER_PUCCH_N", typeof(string)),
                        new DataColumn("LTE_UE_PHR_PHR", typeof(string)),
                        new DataColumn("LTE_UE_PHR_N", typeof(string)),
                        new DataColumn("LTE_UE_TA_TA", typeof(string)),
                        new DataColumn("LTE_UE_TA_N", typeof(string)),
                        new DataColumn("LTE_UE_ATTENUATION_ATTENUATION", typeof(string)),
                        new DataColumn("LTE_UE_ATTENUATION_N", typeof(string)),
                        new DataColumn("LTE_UE_RSSI_PUSCH_RSSI", typeof(string)),
                        new DataColumn("LTE_UE_RSSI_PUSCH_N", typeof(string)),
                        new DataColumn("LTE_UE_RSSI_PUCCH_RSSI", typeof(string)),
                        new DataColumn("LTE_UE_RSSI_PUCCH_N", typeof(string)),
                        new DataColumn("LTE_UE_PSD_PUSCH_PSD", typeof(string)),
                        new DataColumn("LTE_UE_PSD_PUSCH_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BW_DL_PRBs", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BW_DL_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BW_UL_PRBs", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BW_UL_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_THROUGHPUT_DL_throughput", typeof(string)),
                        new DataColumn("LTE_UE_MAC_THROUGHPUT_DL_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_THROUGHPUT_UL_throughput", typeof(string)),
                        new DataColumn("LTE_UE_MAC_THROUGHPUT_UL_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BLER_DL_BLER", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BLER_DL_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BLER_UL_BLER", typeof(string)),
                        new DataColumn("LTE_UE_MAC_BLER_UL_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_EFFICIENCY_DL_EFFICIENCY", typeof(string)),
                        new DataColumn("LTE_UE_MAC_EFFICIENCY_DL_N", typeof(string)),
                        new DataColumn("LTE_UE_MAC_EFFICIENCY_UL_EFFICIENCY", typeof(string)),
                        new DataColumn("LTE_UE_MAC_EFFICIENCY_UL_N", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_THROUGHPUT_DL_THROUGHPUT", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_THROUGHPUT_DL_N", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_THROUGHPUT_UL_THROUGHPUT", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_THROUGHPUT_UL_N", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_BEARER_DL_BEARER", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_BEARER_DL_N", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_BEARER_UL_BEARER", typeof(string)),
                        new DataColumn("LTE_UE_PDCP_BEARER_UL_N", typeof(string)),
                        new DataColumn("LTE_UE_ATTACH_SUCCESS_N_ATTEMPT", typeof(string)),
                        new DataColumn("LTE_UE_ATTACH_SUCCESS_N_SUCCESS", typeof(string)),
                        new DataColumn("LTE_UE_ATTACH_FAILURE_N_ATTEMPT", typeof(string)),
                        new DataColumn("LTE_UE_ATTACH_FAILURE_N_FAILURE_noresponse", typeof(string)),
                        new DataColumn("LTE_UE_ATTACH_FAILURE_N_FAILURE_REJECT", typeof(string)),
                        new DataColumn("LTE_UE_ATTACH_FAILURE_N_FAILURE_UE", typeof(string)),


                        }
                );

                ds.Tables.Add(dt);

                //mySqlDataAdapterSelect.Fill(ds, "5gdetail");

                var mySqlDataAdapter = new MySqlDataAdapter();

                mySqlDataAdapter.InsertCommand = new MySqlCommand(Command, mConnection);


                //mySqlDataAdapter.InsertCommand.Parameters.Add("@PmGroup", MySqlDbType.VarChar, 1500, "PmGroup");
                //mySqlDataAdapter.InsertCommand.Parameters.Add("@jobid", MySqlDbType.VarChar, 500, "jobid");
                //mySqlDataAdapter.InsertCommand.Parameters.Add("@managedElement", MySqlDbType.VarChar, 5000, "managedElement");
                //mySqlDataAdapter.InsertCommand.Parameters.Add("@measObjLdn", MySqlDbType.VarChar, 5000, "measObjLdn");
                //mySqlDataAdapter.InsertCommand.Parameters.Add("@counter", MySqlDbType.VarChar, 5000, "counter");
                //mySqlDataAdapter.InsertCommand.Parameters.Add("@CounterValue", MySqlDbType.VarChar, 5000, "CounterValue");


                mySqlDataAdapter.InsertCommand.Parameters.Add("@id", MySqlDbType.VarChar, 1500, "id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@object_id", MySqlDbType.VarChar, 1500, "object_id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@session_id", MySqlDbType.VarChar, 1500, "session_id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@attribute_id", MySqlDbType.VarChar, 1500, "attribute_id");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@period_from", MySqlDbType.DateTime, 1500, "period_from");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@period_to", MySqlDbType.DateTime, 1500, "period_to");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_GPS_longitude", MySqlDbType.VarChar, 1500, "LTE_UE_GPS_longitude");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_GPS_latitude", MySqlDbType.VarChar, 1500, "LTE_UE_GPS_latitude");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_GPS_height", MySqlDbType.VarChar, 1500, "LTE_UE_GPS_height");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_RAT", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_RAT");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_DUPLEX", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_DUPLEX");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_BAND", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_BAND");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_CARRIER", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_CARRIER");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_FREQUENCY", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_FREQUENCY");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_ENODEB", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_ENODEB");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_CELL", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_CELL");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_SERVER_PCI", MySqlDbType.VarChar, 1500, "LTE_UE_SERVER_PCI");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSRP_RSRP", MySqlDbType.VarChar, 1500, "LTE_UE_RSRP_RSRP");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSRP_N", MySqlDbType.VarChar, 1500, "LTE_UE_RSRP_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSRQ_RSRQ", MySqlDbType.VarChar, 1500, "LTE_UE_RSRQ_RSRQ");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSRQ_N", MySqlDbType.VarChar, 1500, "LTE_UE_RSRQ_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_L_L", MySqlDbType.VarChar, 1500, "LTE_UE_L_L");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_L_N", MySqlDbType.VarChar, 1500, "LTE_UE_L_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_IC_IC", MySqlDbType.VarChar, 1500, "LTE_UE_IC_IC");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_IC_N", MySqlDbType.VarChar, 1500, "LTE_UE_IC_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSSNR_RSSNR", MySqlDbType.VarChar, 1500, "LTE_UE_RSSNR_RSSNR");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSSNR_N", MySqlDbType.VarChar, 1500, "LTE_UE_RSSNR_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_NF_UE_NF", MySqlDbType.VarChar, 1500, "LTE_UE_NF_UE_NF");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_NF_UE_NF_N", MySqlDbType.VarChar, 1500, "LTE_UE_NF_UE_NF_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_CQI_CQI", MySqlDbType.VarChar, 1500, "LTE_UE_CQI_CQI");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_CQI_N", MySqlDbType.VarChar, 1500, "LTE_UE_CQI_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RI_RI", MySqlDbType.VarChar, 1500, "LTE_UE_RI_RI");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RI_N", MySqlDbType.VarChar, 1500, "LTE_UE_RI_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_UTILIZATION_DL_UTILIZAYION", MySqlDbType.VarChar, 1500, "LTE_UE_UTILIZATION_DL_UTILIZAYION");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_UTILIZATION_DL_N", MySqlDbType.VarChar, 1500, "LTE_UE_UTILIZATION_DL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_TX_POWER_PUSCH_TXPOWER", MySqlDbType.VarChar, 1500, "LTE_UE_TX_POWER_PUSCH_TXPOWER");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_TX_POWER_PUSCH_N", MySqlDbType.VarChar, 1500, "LTE_UE_TX_POWER_PUSCH_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_TX_POWER_PUCCH_TXPOWER", MySqlDbType.VarChar, 1500, "LTE_UE_TX_POWER_PUCCH_TXPOWER");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_TX_POWER_PUCCH_N", MySqlDbType.VarChar, 1500, "LTE_UE_TX_POWER_PUCCH_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PHR_PHR", MySqlDbType.VarChar, 1500, "LTE_UE_PHR_PHR");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PHR_N", MySqlDbType.VarChar, 1500, "LTE_UE_PHR_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_TA_TA", MySqlDbType.VarChar, 1500, "LTE_UE_TA_TA");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_TA_N", MySqlDbType.VarChar, 1500, "LTE_UE_TA_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTENUATION_ATTENUATION", MySqlDbType.VarChar, 1500, "LTE_UE_ATTENUATION_ATTENUATION");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTENUATION_N", MySqlDbType.VarChar, 1500, "LTE_UE_ATTENUATION_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSSI_PUSCH_RSSI", MySqlDbType.VarChar, 1500, "LTE_UE_RSSI_PUSCH_RSSI");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSSI_PUSCH_N", MySqlDbType.VarChar, 1500, "LTE_UE_RSSI_PUSCH_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSSI_PUCCH_RSSI", MySqlDbType.VarChar, 1500, "LTE_UE_RSSI_PUCCH_RSSI");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_RSSI_PUCCH_N", MySqlDbType.VarChar, 1500, "LTE_UE_RSSI_PUCCH_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PSD_PUSCH_PSD", MySqlDbType.VarChar, 1500, "LTE_UE_PSD_PUSCH_PSD");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PSD_PUSCH_N", MySqlDbType.VarChar, 1500, "LTE_UE_PSD_PUSCH_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BW_DL_PRBs", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BW_DL_PRBs");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BW_DL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BW_DL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BW_UL_PRBs", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BW_UL_PRBs");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BW_UL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BW_UL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_THROUGHPUT_DL_throughput", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_THROUGHPUT_DL_throughput");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_THROUGHPUT_DL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_THROUGHPUT_DL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_THROUGHPUT_UL_throughput", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_THROUGHPUT_UL_throughput");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_THROUGHPUT_UL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_THROUGHPUT_UL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BLER_DL_BLER", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BLER_DL_BLER");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BLER_DL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BLER_DL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BLER_UL_BLER", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BLER_UL_BLER");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_BLER_UL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_BLER_UL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_EFFICIENCY_DL_EFFICIENCY", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_EFFICIENCY_DL_EFFICIENCY");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_EFFICIENCY_DL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_EFFICIENCY_DL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_EFFICIENCY_UL_EFFICIENCY", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_EFFICIENCY_UL_EFFICIENCY");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_MAC_EFFICIENCY_UL_N", MySqlDbType.VarChar, 1500, "LTE_UE_MAC_EFFICIENCY_UL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_THROUGHPUT_DL_THROUGHPUT", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_THROUGHPUT_DL_THROUGHPUT");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_THROUGHPUT_DL_N", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_THROUGHPUT_DL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_THROUGHPUT_UL_THROUGHPUT", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_THROUGHPUT_UL_THROUGHPUT");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_THROUGHPUT_UL_N", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_THROUGHPUT_UL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_BEARER_DL_BEARER", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_BEARER_DL_BEARER");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_BEARER_DL_N", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_BEARER_DL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_BEARER_UL_BEARER", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_BEARER_UL_BEARER");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_PDCP_BEARER_UL_N", MySqlDbType.VarChar, 1500, "LTE_UE_PDCP_BEARER_UL_N");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTACH_SUCCESS_N_ATTEMPT", MySqlDbType.VarChar, 1500, "LTE_UE_ATTACH_SUCCESS_N_ATTEMPT");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTACH_SUCCESS_N_SUCCESS", MySqlDbType.VarChar, 1500, "LTE_UE_ATTACH_SUCCESS_N_SUCCESS");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTACH_FAILURE_N_ATTEMPT", MySqlDbType.VarChar, 1500, "LTE_UE_ATTACH_FAILURE_N_ATTEMPT");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTACH_FAILURE_N_FAILURE_noresponse", MySqlDbType.VarChar, 1500, "LTE_UE_ATTACH_FAILURE_N_FAILURE_noresponse");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTACH_FAILURE_N_FAILURE_REJECT", MySqlDbType.VarChar, 1500, "LTE_UE_ATTACH_FAILURE_N_FAILURE_REJECT");
                mySqlDataAdapter.InsertCommand.Parameters.Add("@LTE_UE_ATTACH_FAILURE_N_FAILURE_UE", MySqlDbType.VarChar, 1500, "LTE_UE_ATTACH_FAILURE_N_FAILURE_UE");


                mySqlDataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                for (int i = 0; i < lstFileInfo.Count - 1; i++)
                {
                    DataRow row = ds.Tables[0].NewRow();
                    //row["PmGroup"] = objListFileDetail[i].PmGroup;
                    //row["jobid"] = objListFileDetail[i].jobId;
                    //row["managedElement"] = objListFileDetail[i].managedElement;
                    //row["measObjLdn"] = objListFileDetail[i].measObjLdn;
                    //row["counter"] = objListFileDetail[i].counter;
                    //row["CounterValue"] = objListFileDetail[i].CounterValue;
                    //ds.Tables[0].Rows.Add(row);

                    //row["id"] = lstFileInfo[i].object_id;
                    //row["object_id"] = lstFileInfo[i].object_id == ""? null : lstFileInfo[i].object_id;
                    //row["session_id"] = lstFileInfo[i].session_id;
                    row["attribute_id"] = attributeId;
                    row["period_from"] = lstFileInfo[i].period_from;// == null :"" : lstFileInfo[i].sTimeStemp;
                    row["period_to"] = lstFileInfo[i].period_to;
                    row["LTE_UE_GPS_longitude"] = lstFileInfo[i].LTE_UE_GPS_Longitude;
                    row["LTE_UE_GPS_latitude"] = lstFileInfo[i].LTE_UE_GPS_Latitude;
                    row["LTE_UE_GPS_height"] = lstFileInfo[i].LTE_UE_GPS_Height;
                    row["LTE_UE_SERVER_RAT"] = lstFileInfo[i].LTE_UE_SERVER_RAT;
                    row["LTE_UE_SERVER_DUPLEX"] = lstFileInfo[i].LTE_UE_SERVER_duplex;
                    row["LTE_UE_SERVER_BAND"] = lstFileInfo[i].LTE_UE_SERVER_band;
                    row["LTE_UE_SERVER_CARRIER"] = lstFileInfo[i].LTE_UE_SERVER_carrier;
                    row["LTE_UE_SERVER_FREQUENCY"] = lstFileInfo[i].LTE_UE_SERVER_frequency;
                    row["LTE_UE_SERVER_ENODEB"] = lstFileInfo[i].LTE_UE_SERVER_enodeb;
                    row["LTE_UE_SERVER_CELL"] = lstFileInfo[i].LTE_UE_SERVER_cell;
                    row["LTE_UE_SERVER_PCI"] = lstFileInfo[i].LTE_UE_SERVER_PCI;
                    row["LTE_UE_RSRP_RSRP"] = lstFileInfo[i].LTE_UE_RSRP_RSRP;
                    row["LTE_UE_RSRP_N"] = lstFileInfo[i].LTE_UE_RSRP_N;
                    row["LTE_UE_RSRQ_RSRQ"] = lstFileInfo[i].LTE_UE_RSRQ_RSRQ;
                    row["LTE_UE_RSRQ_N"] = lstFileInfo[i].LTE_UE_RSRQ_N;
                    row["LTE_UE_L_L"] = lstFileInfo[i].LTE_UE_L_L;
                    row["LTE_UE_L_N"] = lstFileInfo[i].LTE_UE_L_N;
                    row["LTE_UE_IC_IC"] = lstFileInfo[i].LTE_UE_IC_IC;
                    row["LTE_UE_IC_N"] = lstFileInfo[i].LTE_UE_IC_N;
                    row["LTE_UE_RSSNR_RSSNR"] = lstFileInfo[i].LTE_UE_RSSNR_RSSNR;
                    row["LTE_UE_RSSNR_N"] = lstFileInfo[i].LTE_UE_RSSNR_N;
                    row["LTE_UE_NF_UE_NF"] = lstFileInfo[i].LTE_UE_NF_UE_NF;
                    row["LTE_UE_NF_UE_NF_N"] = lstFileInfo[i].LTE_UE_NF_UE_N;
                    row["LTE_UE_CQI_CQI"] = lstFileInfo[i].LTE_UE_CQI_CQI;
                    row["LTE_UE_CQI_N"] = lstFileInfo[i].LTE_UE_CQI_N;
                    row["LTE_UE_RI_RI"] = lstFileInfo[i].LTE_UE_RI_RI;
                    row["LTE_UE_RI_N"] = lstFileInfo[i].LTE_UE_RI_N;
                    row["LTE_UE_UTILIZATION_DL_UTILIZAYION"] = lstFileInfo[i].LTE_UE_UTILIZATION_DL_utilization;
                    row["LTE_UE_UTILIZATION_DL_N"] = lstFileInfo[i].LTE_UE_UTILIZATION_DL_N;
                    row["LTE_UE_TX_POWER_PUSCH_TXPOWER"] = lstFileInfo[i].LTE_UE_TX_POWER_PUSCH_TXpower;
                    row["LTE_UE_TX_POWER_PUSCH_N"] = lstFileInfo[i].LTE_UE_TX_POWER_PUSCH_N;
                    row["LTE_UE_TX_POWER_PUCCH_TXPOWER"] = lstFileInfo[i].LTE_UE_TX_POWER_PUCCH_TXpower;
                    row["LTE_UE_TX_POWER_PUCCH_N"] = lstFileInfo[i].LTE_UE_TX_POWER_PUCCH_N;
                    row["LTE_UE_PHR_PHR"] = lstFileInfo[i].LTE_UE_PHR_PHR;
                    row["LTE_UE_PHR_N"] = lstFileInfo[i].LTE_UE_PHR_N;
                    row["LTE_UE_TA_TA"] = lstFileInfo[i].LTE_UE_TA_TA;
                    row["LTE_UE_TA_N"] = lstFileInfo[i].LTE_UE_TA_N;
                    row["LTE_UE_ATTENUATION_ATTENUATION"] = lstFileInfo[i].LTE_UE_ATTENUATION_attenuation;
                    row["LTE_UE_ATTENUATION_N"] = lstFileInfo[i].LTE_UE_ATTENUATION_N;
                    row["LTE_UE_RSSI_PUSCH_RSSI"] = lstFileInfo[i].LTE_UE_RSSI_PUSCH_RSSI;
                    row["LTE_UE_RSSI_PUSCH_N"] = lstFileInfo[i].LTE_UE_RSSI_PUSCH_N;
                    row["LTE_UE_RSSI_PUCCH_RSSI"] = lstFileInfo[i].LTE_UE_RSSI_PUCCH_RSSI;
                    row["LTE_UE_RSSI_PUCCH_N"] = lstFileInfo[i].LTE_UE_RSSI_PUCCH_N;
                    row["LTE_UE_PSD_PUSCH_PSD"] = lstFileInfo[i].LTE_UE_PSD_PUSCH_PSD;
                    row["LTE_UE_PSD_PUSCH_N"] = lstFileInfo[i].LTE_UE_PSD_PUSCH_N;
                    row["LTE_UE_MAC_BW_DL_PRBs"] = lstFileInfo[i].LTE_UE_MAC_BW_DL_PRBs;
                    row["LTE_UE_MAC_BW_DL_N"] = lstFileInfo[i].LTE_UE_MAC_BW_DL_N;
                    row["LTE_UE_MAC_BW_UL_PRBs"] = lstFileInfo[i].LTE_UE_MAC_BW_UL_PRBs;
                    row["LTE_UE_MAC_BW_UL_N"] = lstFileInfo[i].LTE_UE_MAC_BW_UL_N;
                    row["LTE_UE_MAC_THROUGHPUT_DL_throughput"] = lstFileInfo[i].LTE_UE_MAC_THROUGHPUT_DL_throughput;
                    row["LTE_UE_MAC_THROUGHPUT_DL_N"] = lstFileInfo[i].LTE_UE_MAC_THROUGHPUT_DL_N;
                    row["LTE_UE_MAC_THROUGHPUT_UL_throughput"] = lstFileInfo[i].LTE_UE_MAC_THROUGHPUT_UL_throughput;
                    row["LTE_UE_MAC_THROUGHPUT_UL_N"] = lstFileInfo[i].LTE_UE_MAC_THROUGHPUT_UL_N;
                    row["LTE_UE_MAC_BLER_DL_BLER"] = lstFileInfo[i].LTE_UE_MAC_BLER_DL_BLER;
                    row["LTE_UE_MAC_BLER_DL_N"] = lstFileInfo[i].LTE_UE_MAC_BLER_DL_N;
                    row["LTE_UE_MAC_BLER_UL_BLER"] = lstFileInfo[i].LTE_UE_MAC_BLER_UL_BLER;
                    row["LTE_UE_MAC_BLER_UL_N"] = lstFileInfo[i].LTE_UE_MAC_BLER_UL_N;
                    row["LTE_UE_MAC_EFFICIENCY_DL_EFFICIENCY"] = lstFileInfo[i].LTE_UE_MAC_EFFICIENCY_DL_efficiency;
                    row["LTE_UE_MAC_EFFICIENCY_DL_N"] = lstFileInfo[i].LTE_UE_MAC_EFFICIENCY_DL_N;
                    row["LTE_UE_MAC_EFFICIENCY_UL_EFFICIENCY"] = lstFileInfo[i].LTE_UE_MAC_EFFICIENCY_UL_efficiency;
                    row["LTE_UE_MAC_EFFICIENCY_UL_N"] = lstFileInfo[i].LTE_UE_MAC_EFFICIENCY_UL_N;
                    row["LTE_UE_PDCP_THROUGHPUT_DL_THROUGHPUT"] = lstFileInfo[i].LTE_UE_PDCP_THROUGHPUT_DL_throughput;
                    row["LTE_UE_PDCP_THROUGHPUT_DL_N"] = lstFileInfo[i].LTE_UE_PDCP_THROUGHPUT_DL_N;
                    row["LTE_UE_PDCP_THROUGHPUT_UL_THROUGHPUT"] = lstFileInfo[i].LTE_UE_PDCP_THROUGHPUT_UL_throughput;
                    row["LTE_UE_PDCP_THROUGHPUT_UL_N"] = lstFileInfo[i].LTE_UE_PDCP_THROUGHPUT_UL_N;
                    row["LTE_UE_PDCP_BEARER_DL_BEARER"] = lstFileInfo[i].LTE_UE_PDCP_BEARER_DL_bearer;
                    row["LTE_UE_PDCP_BEARER_DL_N"] = lstFileInfo[i].LTE_UE_PDCP_BEARER_DL_N;
                    row["LTE_UE_PDCP_BEARER_UL_BEARER"] = lstFileInfo[i].LTE_UE_PDCP_BEARER_UL_bearer;
                    row["LTE_UE_PDCP_BEARER_UL_N"] = lstFileInfo[i].LTE_UE_PDCP_BEARER_UL_N;
                    row["LTE_UE_ATTACH_SUCCESS_N_ATTEMPT"] = lstFileInfo[i].LTE_UE_ATTACH_SUCCESS_N_attempt;
                    row["LTE_UE_ATTACH_SUCCESS_N_SUCCESS"] = lstFileInfo[i].LTE_UE_ATTACH_SUCCESS_N_success;
                    row["LTE_UE_ATTACH_FAILURE_N_ATTEMPT"] = lstFileInfo[i].LTE_UE_ATTACH_FAILURE_N_attempt;
                    row["LTE_UE_ATTACH_FAILURE_N_FAILURE_noresponse"] = lstFileInfo[i].LTE_UE_ATTACH_FAILURE_N_failure_noresponse;
                    row["LTE_UE_ATTACH_FAILURE_N_FAILURE_REJECT"] = lstFileInfo[i].LTE_UE_ATTACH_FAILURE_N_failure_reject;
                    row["LTE_UE_ATTACH_FAILURE_N_FAILURE_UE"] = lstFileInfo[i].LTE_UE_ATTACH_FAILURE_N_failure_UE;
                    ds.Tables[0].Rows.Add(row);

                }


                mySqlDataAdapter.UpdateBatchSize = 100;
                mySqlDataAdapter.Update(ds, "table1");

                transaction.Commit();

            }




            //objFileInfo.Add(objFIleHeader);


        }




    }
}
