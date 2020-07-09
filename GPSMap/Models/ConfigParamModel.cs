using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace GPSMap.Models
{
    public class ConfigParamModel
    {
        public string Path { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PixelSizeMeters { get; set; }
        public int EUARFCN { get; set; }
        public string PLMNObjects { get; set; }
        //public List<GPSDB.Report_ExcelData> ReportExcelData { get; set; }
        public DataTable ReportExcelData { get; set; }
        public string ReportName { get; set; }
        public string IFSDataId { get; set; }
        public string ReportType { get; set; }
        public string GeoReportData { get; set; }
    }
}