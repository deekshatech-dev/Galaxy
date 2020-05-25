//------
//Config
//------

var sInputFilePath = "H:/GTA_TA/Config/LTE_drive_test/20181025.1.nmf";

// Default is today
// var cDate = new Date();
// If specific date is needed, specify it here
// Months indexed from 0 (so January is month 0) to 11
// var cDate = new Date(2019, 0, 11, 0, 0, 0); for 11th January 2019 ZG(YYYY,MM,DD,HH,MM,SS)

var cDate = new Date(2018, 0, 0, 0, 0, 0);

//-------
// Server
//-------

if (defaultConnection.login() == false)
{
  log.error(defaultConnection.lastError());
  fatal(-1);
}

//-------------------------------------
// Creation of snapshot and PLMN object
//-------------------------------------

var cSnapshot = defaultConnection.makeSnapshot(cDate, false);
if (cSnapshot == undefined)
{
  log.error("Failed to create snapshot, error: " + defaultConnection.lastError());
  fatal(-1);
}

cSnapshot.addObject("PLMN", "PLMN", "");
cSnapshot.setAttribute("NAME", "PLMN");

//---------------------------
// 1st creation of CSV reader
//---------------------------

var cCsvReader = new CsvReader();
cCsvReader.setCodec("ANSI");
cCsvReader.setFieldSeparator(",");
cCsvReader.setDateTimeFormat("yyyy-MM-dd hh:mm:ss");
cCsvReader.setStrict(false);

if (!cCsvReader.open(sInputFilePath))
{
  log.error("Failed to open input file: " + sInputFilePath);
  fatal(-1);
}

//--------------------
// Creation of session
//--------------------

var cSessionDate = new Date(2018, 9, 25, 0, 0, 0);
cSessionDate.setHours(0);
cSessionDate.setMinutes(0);
cSessionDate.setSeconds(0);
var sSession = "UE_import " + format.date(cSessionDate, "dd-MM-yyyy");

log.info ("Snapshot and session created");

//--------------------------------------------------------------------------------------
// User attributes, number of GPS, channel and cell measurement samples, number of lines
//--------------------------------------------------------------------------------------

var IMSI = "";						// IMSI
var IMEI = "";						// IMEI
var device_name = "";				// Device name
var device_label = "";				// Device label
var measurement_method = "";		// Measurement method (e.g. drive test)
var measurement_system = "";		// Measurement system (e.g. Nemo Outdoor)
var samples_gps = 0;				// Number of GPS measurement samples
var samples_channel = 0;			// Number of channel measurement samples
var samples_cell = 0;				// Number of cell measurement samples
var lines = 0;						// Number of lines

lines = 0;
while (!cCsvReader.isEOF())
{
	cCsvReader.readLine();

	// User attributes

	if (cCsvReader.asString(0) == "#EI")		// Entry 0 = #EI -> entry 3 = IMEI
	IMEI = cCsvReader.asString(3);
	if (cCsvReader.asString(0) == "#SI")		// Entry 0 = #SI -> entry 3 = IMSI
	IMSI = cCsvReader.asString(3);
	if (cCsvReader.asString(0) == "#DN")		// Entry 0 = #DN -> entry 3 = device name
	device_name = cCsvReader.asString(3);
	if (cCsvReader.asString(0) == "#DL")		// Entry 0 = #DL -> entry 3 = device label
	device_label = cCsvReader.asString(3);

	// Count of GPS, cell measurement samples and of lines

	if (cCsvReader.asString(0) == "GPS")		// Entry 0 = GPS
	samples_gps ++;
	if (cCsvReader.asString(0) == "CHI")		// Entry 0 = CHI
	{
		if (cCsvReader.asString(3) == 7 || cCsvReader.asString(3) == 8)		// Entry 3 = RAT (LTE = 7 or 8
		samples_channel ++;
	}
	if (cCsvReader.asString(0) == "CELLMEAS")	// Entry 0 = CELLMEAS
	samples_cell ++;
	lines ++;
}
delete cCsvReader;

cSnapshot.addObject("PLMN" + "/" + IMSI, "USER", "PLMN");
cSnapshot.setAttribute("IMSI", IMSI);
cSnapshot.setAttribute("IMEI", IMEI);
cSnapshot.setAttribute("UE_NAME", device_name);
cSnapshot.setAttribute("UE_LABEL", device_label);
cSnapshot.setAttribute("MEASUREMENT_METHOD", "drive test");
cSnapshot.setAttribute("MEASUREMENT_SYSTEM", "Nemo Outdoor");

//---------------------------
// 2nd creation of CSV reader
//---------------------------

var cCsvReader = new CsvReader();
cCsvReader.setCodec("ANSI");
cCsvReader.setFieldSeparator(",");
cCsvReader.setDateTimeFormat("yyyy-MM-dd hh:mm:ss");
cCsvReader.setStrict(false);

if (!cCsvReader.open(sInputFilePath))
{
  log.error("Failed to open input file: " + sInputFilePath);
  fatal(-1);
}

//--------------------------------------------------------------------------
// Raw GPS and channel (cell ID) data and lines indicating cell measurements
//--------------------------------------------------------------------------

var i = 0;										// Counter
var j = 0;
var k = 0;
var l = 0;
var time_stamp = "";							// Time stamp of any sample as string
var HH = 0;										// Time stamp of any sample as HHMMSS
var MM = 0;
var SS = 0;
var time_gps = new Array (samples_gps);			// Time stamp of GPS sample as decimal
var longitude_gps = new Array (samples_gps);	// Coordinates indicated by GPS samples
var latitude_gps = new Array (samples_gps);
var height_gps = new Array (samples_gps);
var time_channel = new Array (samples_channel);	// Time stamp of channel sample as decimal
var GCI = new Array (samples_channel);			// Global cell IDs indicated by channel samples
var line_cell = new Array (samples_cell);		// Lines indicating cell measurements

while (!cCsvReader.isEOF())
{
	cCsvReader.readLine();

	// Collection of GPS samples

	if (cCsvReader.asString(0) == "GPS")		// Entry 0 = GPS -> Entry 1 = time stamp, Entry 3/4/5 = longitude/latitude/height above sea level
	{
		time_stamp = cCsvReader.asString(1);
		HH = parseFloat (time_stamp.slice (0,2));
		MM = parseFloat (time_stamp.slice (3,5));
		SS = parseFloat (time_stamp.slice (6,12));
        time_gps[i] = HH + MM / 60 + SS / 3600;
		longitude_gps[i] = cCsvReader.asDouble(3);
		latitude_gps[i] = cCsvReader.asDouble(4);
		height_gps[i] = cCsvReader.asDouble(5);
		i ++;
	}

	if (cCsvReader.asString(0) == "CHI")		// Entry 0 = CHI -> Entry 1 = time stamp, Entry 3/4/5 = longitude/latitude/height above sea level
	{
		if (cCsvReader.asString(3) == 7 || cCsvReader.asString(3) == 8)		// Entry 3 = RAT (LTE = 7 or 8
		{
			time_stamp = cCsvReader.asString(1);
			HH = parseFloat (time_stamp.slice (0,2));
			MM = parseFloat (time_stamp.slice (3,5));
			SS = parseFloat (time_stamp.slice (6,12));
			time_channel[j] = HH + MM / 60 + SS / 3600;
			GCI[j] = cCsvReader.asInt(9);
			j ++;
		}
	}

	if (cCsvReader.asString(0) == "CELLMEAS")	// Entry 0 = CELLMEAS -> line indicating cell measurement
	{
		line_cell[k] = l;
		k ++;
	}
	l ++;
}
delete cCsvReader;
log.info ("GPS samples scanned");

//---------------------------
// 3rd creation of CSV reader
//---------------------------

var cCsvReader = new CsvReader();
cCsvReader.setCodec("ANSI");
cCsvReader.setFieldSeparator(",");
cCsvReader.setDateTimeFormat("yyyy-MM-dd hh:mm:ss");
cCsvReader.setStrict(false);

if (!cCsvReader.open(sInputFilePath))
{
  log.error("Failed to open input file: " + sInputFilePath);
  fatal(-1);
}

//-----------------------
// Creation of KPI writer
//-----------------------

var cKPIWriter = defaultConnection.startSession(sSession);
if (cKPIWriter == undefined)
{
  log.error(defaultConnection.lastError);
  fatal(-1);
}

cKPIWriter.objectClass = "PLMN/USER";
cKPIWriter.objectCreation = true;
cKPIWriter.defineClassMapping("PLMN", "PLMN", "$[PLMN]");
cKPIWriter.defineClassMapping("PLMN/USER", "PLMN/USER", "$[PLMN]/$[USER]");

//------------------------------
// KPI definitions and variables
//------------------------------

var time_kpi = "";					// Time stamp of KPI sample as decimal
var time_start = "";				// Start time of 0.5 s interval to which KPI sample is assigned
var time_end = "";					// End time of 0.5 s interval to which KPI sample is assigned

var start_cell = 0;					// Line indicating current cell measurement
var end_cell = 0;					// Line indicating next cell measurement

var longitude_kpi = 0.0;			// Coordinates of KPI sample
var latitude_kpi = 0.0;
var height_kpi = 0.0;
cKPIWriter.defineKPI("LTE_UE_GPS", new Array("longitude","latitude","height"));						// Triple of gauge values

var RAT = 0;						// Radio access technology of serving cell (1/2/3/4 = WLAN/GPRS/HSPA/LTE)
var duplex = 0;						// Duplex mode of serving cell (0/1 = FDD/TDD)
var band = 0;						// ID of serving frequency band
var carrier = 0;					// EUARFCN of serving carrier
var f = 0.0;						// Frequency of serving band
var cellID = 0;						// ID of serving cell
var enodebID = 0;					// ID of serving eNodeB
var PCI = 0;						// PCI of serving cell
cKPIWriter.defineKPI("LTE_UE_SERVER", new Array("RAT","duplex","band","carrier","frequency","enodeb","cell","PCI"));	// Octupke of gauge values

var RSRP = 0.0;						// RSRP exact
var RSRP_round = 0.0;				// RSRP rounded to 0.5 dB granularity
cKPIWriter.defineKPI("LTE_UE_RSRP", new Array("RSRP","N"));											// Histogram

var RSRQ = 0.0;						// RSRQ exact
var RSRQ_round = 0.0;				// RSRQ rounded to 0.5 dB granularity
cKPIWriter.defineKPI("LTE_UE_RSRQ", new Array("RSRQ","N"));											// Histogram

var L = 0.0;						// Path loss exact
var L_round = 0.0;					// Path loss rounded to 0.5 dB granularity
cKPIWriter.defineKPI("LTE_UE_L", new Array("L","N"));												// Histogram

var C = 0.0;						// RSRP of server absolute
var I = 0.0;						// RSRP summed over all neighbors absolute
var IC = 0.0;						// I / C exact
var IC_round = 0.0;					// I / C rounded to 0.1 granularity
cKPIWriter.defineKPI("LTE_UE_IC", new Array("IC","N"));												// Histogram

var RSSNR = 0.0;					// RSSNR exact
var RSSNR_round = 0.0;				// RSSNR rounded to 0.5 dB granularity
var nf = 0.0;						// UE noise figure
cKPIWriter.defineKPI("LTE_UE_RSSNR", new Array("RSSNR","N"));										// Histogram
cKPIWriter.defineKPI("LTE_UE_NF_UE", new Array("NF","N"));											// Histogram

var cqi = 0;						// CQI
var ri = 0;							// (Maximum) RI
var utilization = 0.0;				// PRB utilization
cKPIWriter.defineKPI("LTE_UE_CQI", new Array("CQI","N"));											// Histogram
cKPIWriter.defineKPI("LTE_UE_RI", new Array("RI","N"));												// Histogram
cKPIWriter.defineKPI("LTE_UE_UTILIZATION_DL", new Array("utilization","N"));						// Histogram

var tx_pusch = 0.0;					// UE power on PUSCH
var tx_pucch = 0.0;					// UE power on PUCCH
var phr = 0.0;						// UE power headroom
cKPIWriter.defineKPI("LTE_UE_TX_POWER_PUSCH", new Array("TXpower","N"));							// Histogram
cKPIWriter.defineKPI("LTE_UE_TX_POWER_PUCCH", new Array("TXpower","N"));							// Histogram
cKPIWriter.defineKPI("LTE_UE_PHR", new Array("PHR","N"));											// Histogram

var ta = 0.0;						// Timing advance exact
var ta_round = 0.0;					// Timing advance rounded to granularity 1 m
var attenuation = 0.0;				// Path loss per km
cKPIWriter.defineKPI("LTE_UE_TA", new Array("TA","N"));												// Histogram
cKPIWriter.defineKPI("LTE_UE_ATTENUATION", new Array("attenuation","N"));							// Histogram

var rssi_pusch = 0.0;				// RSSI on PUSCH exact;
var rssi_pusch_round = 0.0;			// RSSI on PUSCH rounded to granularity 0.5 dBm
var rssi_pucch = 0.0;				// RSSI on PUCCH
var psd_pusch = 0.0;				// Power spectral density on PUSCH
cKPIWriter.defineKPI("LTE_UE_RSSI_PUSCH", new Array("RSSI","N"));									// Histogram
cKPIWriter.defineKPI("LTE_UE_RSSI_PUCCH", new Array("RSSI","N"));									// Histogram
cKPIWriter.defineKPI("LTE_UE_PSD_PUSCH", new Array("PSD","N"));										// Histogram

var BW_DL = 0.0;					// Bandwidth occupied by UE on DL exakt
var BW_DL_round = 0.0;				// Bandwidth rounded to 1 PRB granularity
var BW_UL = 0.0;					// Bandwidth occupied by UE on UL
var BW_UL_round = 0.0;				// Bandwidth rounded to 1 PRB granularity
cKPIWriter.defineKPI("LTE_UE_MAC_BW_DL", new Array("PRBs","N"));									// Histogram
cKPIWriter.defineKPI("LTE_UE_MAC_BW_UL", new Array("PRBs","N"));									// Histogram

var mac_throughput_DL = 0.0;		// Throughput on MAC layer on DL
var mac_throughput_UL = 0.0;		// Throughput on MAC layer on UL
var mac_BLER = 0.0;					// Block error ratio on MAC layer
var efficiency = 0.0;				// Spectrum efficiency
cKPIWriter.defineKPI("LTE_UE_MAC_THROUGHPUT_DL", new Array("throughput","N"));						// Histogram
cKPIWriter.defineKPI("LTE_UE_MAC_THROUGHPUT_UL", new Array("throughput","N"));						// Histogram
cKPIWriter.defineKPI("LTE_UE_MAC_BLER_DL", new Array("BLER","N"));									// Histogram
cKPIWriter.defineKPI("LTE_UE_MAC_BLER_UL", new Array("BLER","N"));									// Histogram
cKPIWriter.defineKPI("LTE_UE_MAC_EFFICIENCY_DL", new Array("efficiency","N"));						// Histogram
cKPIWriter.defineKPI("LTE_UE_MAC_EFFICIENCY_UL", new Array("efficiency","N"));						// Histogram

var ppcp_throughput = 0.0;				// Throughput on PDCP layer
var pdcp_bearer = 0;					// Number of parallel PDCP bearers
cKPIWriter.defineKPI("LTE_UE_PDCP_THROUGHPUT_DL", new Array("throughput","N"));						// Histogram
cKPIWriter.defineKPI("LTE_UE_PDCP_THROUGHPUT_UL", new Array("throughput","N"));						// Histogram
cKPIWriter.defineKPI("LTE_UE_PDCP_BEARER_DL", new Array("bearer","N"));								// Histogram
cKPIWriter.defineKPI("LTE_UE_PDCP_BEARER_UL", new Array("bearer","N"));								// Histogram

var time_attach_last = "";				// Time stamp of last attach request
var time_attach_current = "";			// Time stamp of current attach request
var attach_request = false;				// Status set to true if message comes up, status again set to false after termination of procedure
var attach_response = false;			// Status set to true if network responds, status again set to false after new request
var attach_complete = true;				// Status set to true if message comes up, status again set to false after new request
var N_attach = 0;						// Number of attach requests
cKPIWriter.defineKPI("LTE_UE_ATTACH_SUCCESS", new Array("N_attempt","N_success"));					// Ratio
cKPIWriter.defineKPI("LTE_UE_ATTACH_FAILURE", new Array("N_attempt","N_failure_noresponse",
                                                        "N_failure_reject","N_failure_UE"));		// Ratio
// ----------
// KPI import
// ----------

if (!cKPIWriter.startWrite())
{
  log.error(cKPIWriter.lastError());
  fatal(-1);
}

// Lines until 1st cell measurement

for (i = 0;i < line_cell[0];i ++)
cCsvReader.readLine();

for (i = 0;i < samples_cell;i ++)
{
	// Cell measurement = time reference

	cCsvReader.readLine();

	// Time frame

	time_stamp = cCsvReader.asString(1);
	HH = parseFloat (time_stamp.slice (0,2));
	MM = parseFloat (time_stamp.slice (3,5));
	SS = parseFloat (time_stamp.slice (6,12));
	time_kpi = HH + MM / 60 + SS / 3600;
	SS = Math.round (2 * SS - 0.5) / 2;

	time_start = cSessionDate;
	time_start.setHours(HH);
	time_start.setMinutes(MM);
	time_start.setSeconds(SS);
	time_end = cSessionDate;
	time_end.setHours(HH);
	time_end.setMinutes(MM);
	time_end.setSeconds(SS);

	cKPIWriter.object = "PLMN" + "/" + IMSI;
	cKPIWriter.timeFrom = time_start;
	cKPIWriter.timeTo = time_end;

	// GPS coordinates for time frame of KPI sample (linear interpolation between time stamps of closest GPS samples)

	for (j = 0;time_gps[j] < time_kpi && j < samples_gps;j ++);
	longitude_kpi = longitude_gps[j-1] + (time_kpi - time_gps[j-1]) / (time_gps[j] - time_gps[j-1]) * (longitude_gps[j] - longitude_gps[j-1]);
	latitude_kpi = latitude_gps[j-1] + (time_kpi - time_gps[j-1]) / (time_gps[j] - time_gps[j-1]) * (latitude_gps[j] - latitude_gps[j-1]);
	height_kpi = height_gps[j-1] + (time_kpi - time_gps[j-1]) / (time_gps[j] - time_gps[j-1]) * (height_gps[j] - height_gps[j-1]);

	cKPIWriter.addKPIValue("LTE_UE_GPS.longitude", longitude_kpi);
	cKPIWriter.addKPIValue("LTE_UE_GPS.latitude", latitude_kpi);
	cKPIWriter.addKPIValue("LTE_UE_GPS.height", height_kpi);

	// Radio access technology

	RAT = cCsvReader.asInt(3);														// Entry 3 = RAT
	if (RAT == 7 || RAT == 8)														// LTE = 7 or 8 -> set to 4
	RAT = 4;
	if (RAT == 5)																	// HSPA = 5 -> set to 3
	RAT = 3;
	if (RAT == 1)																	// GPRS = 1 -> set to 2
	RAT = 2;
	if (RAT == 20)																	// WLAN = 20 -> set to 1
	RAT = 1;

	cKPIWriter.addKPIValue("LTE_UE_SERVER.RAT", RAT);

	if (RAT == 4)
	{
		// cell ID of serving cell

		for (j = 0;time_channel[j] < time_kpi && j < samples_channel;j ++);
		if (j > 0)
		{
			cellID = GCI[j-1] % 256;												// GCI = 20 bits eNodeB ID + 8 bits cell ID
			enodebID = (GCI[j-1] - cellID) / 256;
			cKPIWriter.addKPIValue("LTE_UE_SERVER.enodeb", enodebID);
			cKPIWriter.addKPIValue("LTE_UE_SERVER.cell", cellID);
		}

		j = cCsvReader.asInt(5);													// Entry 5 = number of cells measured by UE
		I = 0;
		for (k = 0;k < j;k ++)
		{
			if (cCsvReader.asInt(10*k+7) == 0)										// Entry 7,17.. = type of cell (0 = server)
			{
				// IDs of serving cell

				band = cCsvReader.asInt(10*k+8);									// Entry 8,18.. = frequency band
				if (band < 80000)													// Frequency band ID < 80000 -> FDD = 0
				duplex = 0;
				else																// Sonst -> TDD = 1
				duplex = 1;

				if (band == 70064)
				f = 430;
				if (band == 70031 || band == 70072 || band == 70073)
				f = 450;
				if (band == 70071)
				f = 600;
				if (band == 70012 || band == 70013 || band == 70014 || band == 70028 || band == 70029 || band == 70067 || band == 70068 || band == 70085)
				f = 700;
				if (band == 80044)
				f = 750;
				if (band == 70020 || band == 70027)
				f = 800;
				if (band == 70005 || band == 70006 || band == 70018 || band == 70019 || band == 70026)
				f = 850;
				if (band == 70008)
				f = 900;
				if (band == 70011 || band == 80045 || band == 80050 || band == 80051 || band == 80061 || band == 80087)
				f = 1400;
				if (band == 70021 || band == 70024 || band == 70032 || band == 70074 || band == 70075 || band == 70076)
				f = 1500;
				if (band == 70003 || band == 70009 || band == 80035 || band == 80062 || band == 80088)
				f = 1800;
				if (band == 70002 || band == 70025 || band == 80033 || band == 80036 || band == 80037 || band == 80039)
				f = 1900;
				if (band == 80034)
				f = 2000;
				if (band == 70001 || band == 70004 || band == 70010 || band == 70065 || band == 70066 || band == 70070)
				f = 2100;
				if (band == 70023)
				f = 2200;
				if (band == 70030 || band == 80040)
				f = 2350;
				if (band == 70069)
				f = 2500;
				if (band == 70007 || band == 80038 || band == 80041)
				f = 2600;
				if (band == 80052)
				f = 3350;
				if (band == 70022 || band == 80042)
				f = 3500;
				if (band == 70250 || band == 80048 || band == 80049)
				f = 3625;
				if (band == 80043)
				f = 3700;
				if (band == 70252)
				f = 5200;
				if (band == 70240 || band == 80046)
				f = 5540;
				if (band == 70255)
				f = 5700;
				if (band == 80047)
				f = 5900;

				carrier = cCsvReader.asInt(10*k+9);									// Entry 9,19.. = carrier
				PCI = cCsvReader.asInt(10*k+10);									// Entry 10,20.. = PCI

				cKPIWriter.addKPIValue("LTE_UE_SERVER.duplex", duplex);
				cKPIWriter.addKPIValue("LTE_UE_SERVER.band", band);
				cKPIWriter.addKPIValue("LTE_UE_SERVER.carrier", carrier);
				cKPIWriter.addKPIValue("LTE_UE_SERVER.frequency", f);
				cKPIWriter.addKPIValue("LTE_UE_SERVER.PCI", PCI);

				// RSRP, RSRQ and L of serving cell

				RSRP = cCsvReader.asDouble(10*k+12);								// Entry 12,22.. = RSRP
				C = Math.pow (10,RSRP/10);
				RSRP_round = Math.round (2 * RSRP) / 2;								// RSRP given with granularity 0.5 dB
				RSRQ = cCsvReader.asDouble(10*k+13);								// Entry 13,23.. = RSRP
				RSRQ_round = Math.round (2 * RSRQ) / 2;								// RSRQ given with granularity 0.5 dB
				L = cCsvReader.asDouble(10*k+15);									// Entry 15,25.. = path loss
				L_round = Math.round (2 * L) / 2;									// Path loss given with granularity 0.5 dB

				cKPIWriter.addKPIValue("LTE_UE_RSRP.RSRP", RSRP_round);
				cKPIWriter.addKPIValue("LTE_UE_RSRP.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_RSRQ.RSRQ", RSRQ_round);
				cKPIWriter.addKPIValue("LTE_UE_RSRQ.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_L.L", L_round);
				cKPIWriter.addKPIValue("LTE_UE_L.N", 1);
			}

			if (cCsvReader.asInt(10*k+7) == 1 || cCsvReader.asInt(10*k+7) == 2)		// Entry 7,17.. = type of cell (1 or 2 = neighbor)
			{
				RSRP = cCsvReader.asDouble(10*k+12);								// Entry 12,22.. = RSRP
				I += Math.pow (10,RSRP/10);
			}
		}

		// I / C of serving cell

		IC = I / C;
		IC_round = Math.round (10 * IC) / 10;										// I/C given with granularity 0.1
		if (IC_round > 10)															// I/C limited to [0,10]
		IC_round = 10;

		cKPIWriter.addKPIValue("LTE_UE_IC.IC", IC_round);
		cKPIWriter.addKPIValue("LTE_UE_IC.N", 1);
	}

	// Line domain to next cell measurement

	start_cell = line_cell[i] + 1;
	if (i < samples_cell - 1)
	end_cell = line_cell[i+1];
	else
	end_cell = lines;

	// Scan of all lines to next cell measurement

	for (j = start_cell;j < end_cell;j ++)
	{
		cCsvReader.readLine();
		RAT = cCsvReader.asInt(3);													// Entry 3 = RAT

		// Carrier to interference measurement

		if (cCsvReader.asString(0) == "CI")											// Entry 0 = CI
		{
			if ((RAT == 7 || RAT == 8) && cCsvReader.asInt(6) == 0)					// LTE = 7 or 8, entry 6 = type of cell (0 = server)
			{
				// RSSNR of serving cell

				RSSNR = cCsvReader.asDouble(5);										// Entry 5 = RSSNR
				RSSNR_round = Math.round (2 * RSSNR) / 2;							// RSSNR given with granularity 0.5 dB

				cKPIWriter.addKPIValue("LTE_UE_RSSNR.RSSNR", RSSNR_round);
				cKPIWriter.addKPIValue("LTE_UE_RSSNR.N", 1);

				// DL utilization of serving cell

				ri = cCsvReader.asDouble(7);										// Entry 7 = maximum RI (number of Tx antennas)
																					// Utiliaztion given with granularity 1 %
				utilization = Math.round (100 * (Math.pow (10,(RSRP - RSRQ) / 10) - Math.pow (10,-12.6)) / (12 * ri * (1 + IC) * Math.pow (10,RSRP/10)));
				if (utilization < 0)												// Utilization limited to [0..100]
				utilization = 0;
				if (utilization > 100)
				utilization = 100;

				cKPIWriter.addKPIValue("LTE_UE_UTILIZATION_DL.utilization", utilization);
				cKPIWriter.addKPIValue("LTE_UE_UTILIZATION_DL.N", 1);

				// UE receiver noise (within isolated cells with RSRP < -115 dBm only)

				if (RSRP < -115 && IC < 0.1)
				{
					nf = Math.round (2 * (RSRP - RSSNR - 132.2)) / 2;				// Noise figure given with granularity 0.5 dB
					if (nf < 0)														// Noise figure limited to [0,infinity] dB
					nf = 0;

					cKPIWriter.addKPIValue("LTE_UE_UTILIZATION_DL.utilization", utilization);
					cKPIWriter.addKPIValue("LTE_UE_UTILIZATION_DL.N", 1);
				}
			}
		}

		// UE power control measurement

		if (cCsvReader.asString(0) == "TXPC")										// Entry 0 = TXPC
		{
			if (RAT == 7 || RAT == 8)												// LTE = 7 or 8
			{
				// UE power on PUSCH and PUCCH and UE power headroom for serving cell

				tx_pusch = cCsvReader.asDouble(4);									// Entry 4 = UE power on PUSCH
				tx_pusch = Math.round (2 * tx_pusch) / 2;							// UE power given with granularity 0.5 dB
				tx_pucch = cCsvReader.asDouble(5);									// Entry 5 = UE power on PUSCH
				tx_pucch = Math.round (2 * tx_pucch) / 2;							// UE power given with granularity 0.5 dB

				cKPIWriter.addKPIValue("LTE_UE_TX_POWER_PUSCH.TXpower", tx_pusch);
				cKPIWriter.addKPIValue("LTE_UE_TX_POWER_PUSCH.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_TX_POWER_PUCCH.TXpower", tx_pucch);
				cKPIWriter.addKPIValue("LTE_UE_TX_POWER_PUCCH.N", 1);

				if (cCsvReader.asString(6) != "")									// Entry 6 = UE power headroom not always indicated
				{
					phr = cCsvReader.asDouble(6);
					phr = Math.round (2 * phr) / 2;									// UE power headroom given with granularity 0.5 dB

					cKPIWriter.addKPIValue("LTE_UE_PHR.PHR", phr);
					cKPIWriter.addKPIValue("LTE_UE_PHR.N", 1);
				}

				// RSSI on PUSCH and PUCCH for serving cell

				rssi_pusch = tx_pusch - L;
				rssi_pusch_round = Math.round (2 * rssi_pusch) / 2;					// RSSI given with granularity 0.5 dB
				rssi_pucch = Math.round (2 * (tx_pucch - L)) / 2;					// RSSI given with granularity 0.5 dB

				cKPIWriter.addKPIValue("LTE_UE_RSSI_PUSCH.RSSI", rssi_pusch_round);
				cKPIWriter.addKPIValue("LTE_UE_RSSI_PUSCH.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_RSSI_PUCCH.RSSI", rssi_pucch);
				cKPIWriter.addKPIValue("LTE_UE_RSSI_PUCCH.N", 1);
			}
		}

		// Timing advance measurement

		if (cCsvReader.asString(0) == "TAD")										// Entry 0 = TAD
		{
			if ((RAT == 7 || RAT == 8) && cCsvReader.asInt(5) == 0)					// LTE = 7 or 8, entry 5 = type of cell (0 = server)
			{
				// Timing advance

				ta = 78.12 * cCsvReader.asDouble(4);								// Entry 4 = TA
				ta_round = Math.round (ta);											// TA given with granularity 1 m

				cKPIWriter.addKPIValue("LTE_UE_TA.TA", ta);
				cKPIWriter.addKPIValue("LTE_UE_TA.N", 1);

				// Path loss per km
				
				attenuation = Math.round (2000 * (L_round - 60) / ta) / 2;			// Path loss per km given with granularity 0.5 dB / km, 60 dB loss on first 0.1 lambda

				cKPIWriter.addKPIValue("LTE_UE_ATTENUATION.attenuation", attenuation);
				cKPIWriter.addKPIValue("LTE_UE_ATTENUATION.N", 1);
			}
		}

		// Link adaptation measurement DL

		if (cCsvReader.asString(0) == "PLAID")										// Entry 0 = PLAID
		{
			if ((RAT == 7 || RAT == 8) && cCsvReader.asInt(9) == 0)					// LTE = 7 or 8, entry 9 = type of cell (0 = server)
			{
				// Bandwidth occupied by UE in serving cell

				var LA = cCsvReader.asInt(10);										// Entry 10 = number of link adaptation sets LA (each set again has 9 entries)
				var PRB = cCsvReader.asInt(12+9*LA);								// Entry 12 + 9 LA = number of PRB sets PRB (each set again has 3 entries)
				BW_DL = 0.0;
				var weight = 0.0;
				for (k = 0;k < PRB;k ++)											// Entry 14 + 9 LA + 3 PRB = percentage specific bandwidth per TTI is coming up
				{																	// Entry 15 + 9 LA + 3 PRB = specific bandwidth per TTI
					if (cCsvReader.asString(14+9*LA+3*k) != "" && cCsvReader.asString(15+9*LA+3*k) != "")
					{
						var N = cCsvReader.asInt(15+9*LA+3*k);
						if (N > 0)
						{
							BW_DL += N * cCsvReader.asDouble(14+9*LA+3*k);
							weight += cCsvReader.asDouble(14+9*LA+3*k);
						}
					}
				}
				if (weight > 0)
				{
					BW_DL /= weight;
					BW_DL_round = Math.round (BW_DL);								// Average bandwidth given with granularity 1 PRB

					cKPIWriter.addKPIValue("LTE_UE_MAC_BW_DL.PRBs", BW_DL_round);
					cKPIWriter.addKPIValue("LTE_UE_MAC_BW_DL.N", 1);
				}
			}
		}

		// CQI measurement

		if (cCsvReader.asString(0) == "CQI")										// Entry 0 = CQI
		{
			if ((RAT == 7 || RAT == 8) &&
				cCsvReader.asInt(5) > 0 && cCsvReader.asInt(12) == 0)				// LTE = 7 or 8, entry 5 = duration of CQI sampling, entry 12 = type of cell (0 = server)
			{
				// CQI of serving cell

				cqi = cCsvReader.asInt(7);											// Entry 7 = CQI
				cKPIWriter.addKPIValue("LTE_UE_CQI.CQI", cqi);
				cKPIWriter.addKPIValue("LTE_UE_CQI.N", cCsvReader.asInt(5));

				// RI of serving cell

				ri = cCsvReader.asInt(13);											// Entry 13 = maximum RI
				for (k = 1;k <= ri;k ++)
				{
					cKPIWriter.addKPIValue("LTE_UE_RI.RI", k);						// Entry 15,17... = percentage of samples per RI
					cKPIWriter.addKPIValue("LTE_UE_RI.N", Math.round (cCsvReader.asDouble(13+2*k)));
				}
			}
		}

		// Link adaptation measurement UL

		if (cCsvReader.asString(0) == "PLAIU")										// Entry 0 = PLAIU
		{
			if ((RAT == 7 || RAT == 8) && cCsvReader.asInt(9) == 0)					// LTE = 7 or 8, entry 9 = type of cell (0 = server)
			{
				// Bandwidth occupied by UE in serving cell

				LA = cCsvReader.asInt(10);											// Entry 10 = number of link adaptation sets LA (each set again has 9 entries)
				PRB = cCsvReader.asInt(12+9*LA);									// Entry 12 + 9 LA = number of PRB sets PRB (each set again has 3 entries)
				BW_UL = 0.0;
				weight = 0.0;
				for (k = 0;k < PRB;k ++)											// Entry 14 + 9 LA + 3 PRB = percentage specific bandwidth per TTI is coming up
				{																	// Entry 15 + 9 LA + 3 PRB = specific bandwidth per TTI
					if (cCsvReader.asString(14+9*LA+3*k) != "" && cCsvReader.asString(15+9*LA+3*k) != "")
					{
						N = cCsvReader.asInt(15+9*LA+3*k);
						if (N > 0)
						{
							BW_UL += N * cCsvReader.asDouble(14+9*LA+3*k);
							weight += cCsvReader.asDouble(14+9*LA+3*k);
						}
					}
				}
				if (weight > 0)
				{
					BW_UL /= weight;
					BW_UL_round = Math.round (BW_UL);								// Average bandwidth given with granularity 1 PRB

					cKPIWriter.addKPIValue("LTE_UE_MAC_BW_UL.PRBs", BW_UL_round);
					cKPIWriter.addKPIValue("LTE_UE_MAC_BW_UL.N", 1);
				}

				// Power spectral density on PUSCH for serving cell

				if (BW_UL > 1)
				{
					psd_pusch = Math.round (2 * (rssi_pucch - 4.343 * Math.log (BW_UL))) / 2;	// Power spectral density given with granularity 1 dBm/PRB

					cKPIWriter.addKPIValue("LTE_UE_PSD_PUSCH.PSD", psd_pusch);
					cKPIWriter.addKPIValue("LTE_UE_PSD_PUSCH.N", 1);
				}
			}
		}

		// MAC measurement DL

		if (cCsvReader.asString(0) == "MACRATE")									// Entry 0 = MACRATE
		{
			if ((RAT == 7 || RAT == 8) && cCsvReader.asInt(13) == 0)				// LTE = 7 or 8, entry 13 = type of cell (0 = server)
			{
				// MAC throughput and BLER for serving cell

				mac_BLER = Math.round (cCsvReader.asDouble(6));						// Entry 6 = BLER, given with granularity 1 %
				mac_throughput_DL = Math.round (cCsvReader.asDouble(4) / 1000000);	// Entry 4 = throughput, given with granularity 1 Mbit/s

				cKPIWriter.addKPIValue("LTE_UE_MAC_BLER_DL.BLER", mac_BLER);
				cKPIWriter.addKPIValue("LTE_UE_MAC_BLER_DL.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_MAC_THROUGHPUT_DL.throughput", mac_throughput_DL);
				cKPIWriter.addKPIValue("LTE_UE_MAC_THROUGHPUT_DL.N", 1);

				// Spectrum efficiency for serving cell

				if (BW_DL > 1)
				{
					efficiency = Math.round (10 * mac_throughput_DL / (0.18 * BW_DL)) / 10;	// Spectrum efficiency given with granularity 0.1 Bit/s/Hz

					cKPIWriter.addKPIValue("LTE_UE_MAC_EFFICIENCY_DL.efficiency", efficiency);
					cKPIWriter.addKPIValue("LTE_UE_MAC_EFFICIENCY_DL.N", 1);
				}
			}
		}

		// MAC measurement UL

		if (cCsvReader.asString(0) == "MACRATEU")									// Entry 0 = MACRATEU
		{
			if ((RAT == 7 || RAT == 8) && cCsvReader.asInt(12) == 0)				// LTE = 7 or 8, entry 12 = type of cell (0 = server)
			{
				// MAC throughput and BLER for serving cell

				mac_BLER = Math.round (cCsvReader.asDouble(6));						// Entry 6 = BLER, given with granularity 1 %

				mac_throughput_UL = Math.round (cCsvReader.asDouble(4) / 100000) / 10;	// Entry 4 = throughput, given with granularity 0.1 Mbit/s

				cKPIWriter.addKPIValue("LTE_UE_MAC_BLER_UL.BLER", mac_BLER);
				cKPIWriter.addKPIValue("LTE_UE_MAC_BLER_UL.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_MAC_THROUGHPUT_UL.throughput", mac_throughput_UL);
				cKPIWriter.addKPIValue("LTE_UE_MAC_THROUGHPUT_UL.N", 1);

				// Spectrum efficiency for serving cell

				if (BW_UL > 1)
				{
					efficiency = Math.round (10 * mac_throughput_UL / (0.18 * BW_UL)) / 10;	// Spectrum efficiency given with granularity 0.1 Bit/s/Hz

					cKPIWriter.addKPIValue("LTE_UE_MAC_EFFICIENCY_UL.efficiency", efficiency);
					cKPIWriter.addKPIValue("LTE_UE_MAC_EFFICIENCY_UL.N", 1);
				}
			}
		}

		// PDCP measurement DL

		if (cCsvReader.asString(0) == "PDCPRATED")									// Entry 0 = PDCPRATED
		{
			if (RAT == 7 || RAT == 8)												// LTE = 7 or 8
			{
				// PDCP throughput and number of parallel bearers

				pdcp_throughput = Math.round (cCsvReader.asDouble(5) / 1000000);	// Entry 5 = throughput, given with granularity 1 Mbit/s
				pdcp_bearer = cCsvReader.asInt(7);									// Entry 7 = number of parallel bearers

				cKPIWriter.addKPIValue("LTE_UE_PDCP_THROUGHPUT_DL.throughput", pdcp_throughput);
				cKPIWriter.addKPIValue("LTE_UE_PDCP_THROUGHPUT_DL.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_PDCP_BEARER_DL.bearer", pdcp_bearer);
				cKPIWriter.addKPIValue("LTE_UE_PDCP_BEARER_DL.N", 1);
			}
		}

		// PDCP measurement UL

		if (cCsvReader.asString(0) == "PDCPRATEU")									// Entry 0 = PDCPRATEU
		{
			if (RAT == 7 || RAT == 8)												// LTE = 7 or 8
			{
				// PDCP throughput and number of parallel bearers

				pdcp_throughput = Math.round (cCsvReader.asDouble(5) / 100000) / 10;// Entry 5 = throughput, given with granularity 0.1 Mbit/s
				pdcp_bearer = cCsvReader.asInt(7);									// Entry 7 = number of parallel bearers

				cKPIWriter.addKPIValue("LTE_UE_PDCP_THROUGHPUT_UL.throughput", pdcp_throughput);
				cKPIWriter.addKPIValue("LTE_UE_PDCP_THROUGHPUT_UL.N", 1);
				cKPIWriter.addKPIValue("LTE_UE_PDCP_BEARER_UL.bearer", pdcp_bearer);
				cKPIWriter.addKPIValue("LTE_UE_PDCP_BEARER_UL.N", 1);
			}
		}
	}

	if (!cKPIWriter.commitValues())
	{
		log.error(cKPIWriter.lastError());
		fatal(-1);
	}
}

delete cCsvReader;
log.info ("KPIs imported");

cKPIWriter.endWrite();
delete cKPIWrite;

cSnapshot.commitBuffer();
cSnapshot.save();
delete cSnapshot;