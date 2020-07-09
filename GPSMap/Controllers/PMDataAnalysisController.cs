//using ExcelDataReader;
using GPSMap.Helper;
using GPSMap.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using OfficeOpenXml;
using GPSMap.Data;
using GPSDB;

namespace GPSMap.Controllers
{
    [Authorize]
    public class PMDataAnalysisController : Controller
    {
        // GET: Tab
        public List<IFSDataModel> lstIFSData;
        public ActionResult Explorer()
        {
            TabDetailModel tabDetail = getTabDetails();

            ViewBag.TabHeaderHtml = tabDetail.tabHeaderHtml;
            ViewBag.TabContentHtml = tabDetail.tabContentHtml;
            ViewBag.TabNodes = tabDetail.tabnodes;
            ViewBag.TreeViewPLMNObjectData = GetPLMNObjectData();
            ViewBag.ReportExcelDatajson = JsonConvert.SerializeObject(new List<Report_ExcelData>());

            ConfigParamModel model = new ConfigParamModel();
            model.FromDate = DateTime.Now.AddDays(-7);
            model.ToDate = DateTime.Now;

            return View(model);
        }

        private List<TreeViewNode> getIFSChildNodes(string id, string path)
        {
            List<TreeViewNode> childnodes = null;
            dynamic jsonObj;

            if (lstIFSData.Any(x => x.parentid == id))
            {
                childnodes = new List<TreeViewNode>();
                foreach (IFSDataModel item in lstIFSData.Where(x => x.parentid == id))
                {
                    TreeViewNode node = new TreeViewNode();
                    node.text = item.text;
                    node.href = "#" + item.text;
                    node.isDirectory = item.isDirectory;
                    node.path = path + "/" + item.text;
                    node.data_id = item.data_id;
                    if (!string.IsNullOrEmpty(item.template))
                    {
                        jsonObj = JsonConvert.DeserializeObject(item.template);

                        if (jsonObj["Title"] != null)
                            node.name = jsonObj["Title"];
                        if (jsonObj["Description"] != null)
                            node.desc = jsonObj["Description"];
                        if (jsonObj["Pixmaps"] != null && jsonObj["Pixmaps"].Count > 0 && jsonObj["Pixmaps"][0]["ObjectFilterAttributes"] != null)
                            node.geoFilterAttributes = jsonObj["Pixmaps"][0]["ObjectFilterAttributes"].ToObject<List<string>>();

                    }
                    node.tags = new List<string>() { "0" };
                    node.nodes = getIFSChildNodes(item.id, node.path);

                    childnodes.Add(node);
                }
            }

            return childnodes;
        }

        private string GetPLMNObjectData()
        {

            DBHelper db = new DBHelper();
            //List<TreeViewNode> lstIFSData = db.GetIFSData();
            lstIFSData = db.GetPLMNObjectData();

            List<TreeViewNode> treeNodelist = new List<TreeViewNode>();

            foreach (IFSDataModel item in lstIFSData.Where(x => x.parentid == "0"))
            {
                TreeViewNode node = new TreeViewNode();
                node.text = item.text;
                node.href = "#" + item.text;
                node.tags = new List<string>() { "0" };
                node.nodes = getPLMNObjectChildNodes(item.id);

                //treeNode.nodes.Add(node);
                treeNodelist.Add(node);
            }

            //List<TreeViewNode> treeNodelist = new List<TreeViewNode>();
            //treeNodelist.Add(treeNode);

            string treeViewDatajson = JsonConvert.SerializeObject(treeNodelist);
            return treeViewDatajson;
        }

        private List<TreeViewNode> getPLMNObjectChildNodes(string id)
        {
            List<TreeViewNode> childnodes = null;

            if (lstIFSData.Any(x => x.parentid == id))
            {
                childnodes = new List<TreeViewNode>();
                foreach (IFSDataModel item in lstIFSData.Where(x => x.parentid == id))
                {
                    TreeViewNode node = new TreeViewNode();
                    node.text = item.text;
                    node.href = "#" + item.text;
                    node.tags = new List<string>() { "0" };
                    node.nodes = getPLMNObjectChildNodes(item.id);

                    childnodes.Add(node);
                }
            }

            return childnodes;
        }

        //public ActionResult OpenExe()
        //{
        //    return File("D:\\weekly_reports\\tmp.txt", "text/plain");
        //}

        [HttpPost]
        public ActionResult Calculate(ConfigParamModel form, FormCollection formData)
        {
            ConfigParamModel model = new ConfigParamModel();

            if (form.ReportType.ToLower() == "geo")
            {
                model.GeoReportData = CalculateGeoReports(form, formData);
            }
            else if (form.ReportType.ToLower() == "reports")
            {
                model.ReportExcelData = CalculateReports(form, formData);
            }
            else
            {
                model.GeoReportData = GetReportTemplate(form, formData);
            }

            //return View();
            //return null;

            //return new EmptyResult();
            //return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Ericsson.csv");
            //return File(@"D:\weekly_reports\$2020_$06_$03_Cluster daily.ttr", "text/plain");
            //return File(@"D:\weekly_reports\2020_05_26_Cluster daily.ttr", "text/plain", "ClusterReport.ttr");

            //return File(@"D:\weekly_reports\2020_05_26_Cluster daily.ttr", "text/plain", "ClusterReport.ttr");

            TabDetailModel tabDetail = getTabDetails();

            ViewBag.TabHeaderHtml = tabDetail.tabHeaderHtml;
            ViewBag.TabContentHtml = tabDetail.tabContentHtml;
            ViewBag.TabNodes = tabDetail.tabnodes;
            ViewBag.TreeViewPLMNObjectData = GetPLMNObjectData();

            model.FromDate = DateTime.Now.AddDays(-7);
            model.ToDate = DateTime.Now;

            return View("Explorer", model);
            //return File("D:\\weekly_reports\\tmp.txt", "text/plain");

            //return new HttpStatusCodeResult(204);

        }

        private DataTable CalculateReports(ConfigParamModel form, FormCollection formData)
        {
            DataTable reportExcelData = null;
            string exePath = ConfigurationManager.AppSettings["ReportExePath"];
            string jsonConfigPath = ConfigurationManager.AppSettings["ReportJsonConfigPath"];
            string tempJsonConfigPath = ConfigurationManager.AppSettings["tempJsonConfigPath"];

            DateTime currdt = DateTime.Now;
            string strTimeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff_");
            string newtempJsonConfigPath = tempJsonConfigPath + strTimeStamp + form.ReportType.ToLower() + "_tmp" + ".json";
            string ttrOutputFilePath = string.Empty;
            string ttrOutputFileName = strTimeStamp + form.ReportName + ".ttr";
            string excelOutputFilePath = string.Empty;
            string excelOutputFileName = strTimeStamp + "report_dashboard.xlsx";
            string excelsheetName = string.Empty;

            try
            {
                //create temp json file with changed values
                string json = System.IO.File.ReadAllText(jsonConfigPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                ttrOutputFilePath = jsonObj["Reports"][0]["OutputFile"].ToString() + ttrOutputFileName;
                //excelOutputFilePath = jsonObj["Export"]["XlsxOutput"]["OutputFile"].ToString() + excelOutputFileName;
                excelOutputFilePath = jsonObj["XlsxOutput"]["OutputFile"].ToString() + excelOutputFileName;
                excelsheetName = jsonObj["Reports"][0]["XlsxWorkSheet"].ToString();
                if (form.FromDate != null)
                {
                    jsonObj["Reports"][0]["TimeFrom"] = form.FromDate.Value.Subtract(currdt).Days.ToString();
                }
                if (form.ToDate != null)
                {
                    jsonObj["Reports"][0]["TimeTo"] = form.ToDate.Value.Subtract(currdt).Days.ToString();
                }
                jsonObj["Reports"][0]["Path"] = form.Path;
                jsonObj["Reports"][0]["OutputFile"] = ttrOutputFilePath;
                //OutputFile   $[yyyy]_$[mm]_$[dd]_PLMN weekly.ttr
                //jsonObj["Reports"][0]["Objects"] = JsonConvert.SerializeObject(form.PLMNObjects.Split(','));
                jsonObj["Reports"][0]["Objects"] = Newtonsoft.Json.Linq.JToken.FromObject(form.PLMNObjects.Split(','));

                //jsonObj["Export"]["XlsxOutput"]["OutputFile"] = excelOutputFilePath;
                jsonObj["XlsxOutput"]["OutputFile"] = excelOutputFilePath;

                string output = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                System.IO.File.WriteAllText(newtempJsonConfigPath, output);
                //create temp json file with changed values

                Process myProcess = new Process();
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = exePath;
                //myProcess.StartInfo.Arguments = "\"" + jsonConfigPath + "\"";
                myProcess.StartInfo.Arguments = "-c " + newtempJsonConfigPath;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();

                System.Threading.Thread.Sleep(2000);

                DeleteFile(newtempJsonConfigPath);
            }
            catch (Exception ex)
            {
                DeleteFile(newtempJsonConfigPath);
                throw ex;
            }

            if (!string.IsNullOrEmpty(excelOutputFilePath) && !string.IsNullOrEmpty(excelsheetName))
            {
                //List<Report_ExcelData> dataList = ReadDataFromExcel(excelOutputFilePath, excelsheetName);

                //SaveReportExcelData(dataList);

                //ViewBag.ReportExcelDatajson = JsonConvert.SerializeObject(dataList);

                reportExcelData = ReadDataFromExcel(excelOutputFilePath, excelsheetName, ttrOutputFilePath);

            }

            return reportExcelData;
        }

        //private List<Report_ExcelData> ReadDataFromExcel(string filepath, string sheetName)
        private DataTable ReadDataFromExcel(string excelOutputFilePath, string excelsheetName, string ttrOutputFilePath)
        {
            //var dataList = new List<ReportExcelColumnModel>();
            //List<Report_ExcelData> dataList = new List<Report_ExcelData>();
            DataTable reportExcelData = null;

            bool isCatchFileAccesException = true;
            while (isCatchFileAccesException)
            {
                try
                {
                    // make sure that ttr output file is processed before reading report excel file
                    using (var stream = System.IO.File.Open(ttrOutputFilePath, FileMode.Open, FileAccess.Read))
                    { }

                    using (var stream = System.IO.File.Open(excelOutputFilePath, FileMode.Open, FileAccess.Read))
                    {
                        isCatchFileAccesException = false;
                        //using (var package = new ExcelPackage(file.InputStream))
                        //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(stream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            //var workSheet = currentSheet.First();
                            var workSheet = currentSheet[excelsheetName];
                            ////////
                            //DataTable dt = workSheet.ToDataTable();
                            reportExcelData = workSheet.ToDataTable();
                            //List<DataRow> listOfRows = dt.AsEnumerable().ToList();
                            ////////
                            //var noOfCol = workSheet.Dimension.End.Column;
                            //var noOfRow = workSheet.Dimension.End.Row;
                            //for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            //{
                            //    if (workSheet.Cells[rowIterator, 1].Value != null)
                            //    {
                            //        //var row = new ReportExcelColumnModel();
                            //        Report_ExcelData row = new Report_ExcelData();
                            //        row.Object = Convert.ToString(workSheet.Cells[rowIterator, 1].Value);
                            //        row.PeriodFrom = DateTime.FromOADate(double.Parse(workSheet.Cells[rowIterator, 2].Value.ToString()));
                            //        row.PeriodTo = DateTime.FromOADate(double.Parse(workSheet.Cells[rowIterator, 3].Value.ToString()));
                            //        row.CLUSTER_NAME = Convert.ToString(workSheet.Cells[rowIterator, 4].Value);
                            //        row.PLMN_RRC_estab_succ_rate = GPSMap.Helper.Helper.ToNullableDecimal(workSheet.Cells[rowIterator, 5].Value);
                            //        row.PLMN_RRC_estab_attempt = GPSMap.Helper.Helper.ToNullableDecimal(workSheet.Cells[rowIterator, 6].Value);

                            //        dataList.Add(row);
                            //    }
                            //}
                        }
                    }

                    DeleteFile(ttrOutputFilePath);
                    DeleteFile(excelOutputFilePath);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("The process cannot access the file"))
                    {
                        isCatchFileAccesException = true;
                        System.Threading.Thread.Sleep(500);
                        continue;
                    }
                }
                break;
            }

            return reportExcelData;
        }

        private bool SaveReportExcelData(List<Report_ExcelData> dataList)
        {
            bool isSuccess = false;

            using (var dbContext = new DatabaseContext())
            {
                //var mapAttributes = new MapAttributes();
                //mapAttributes.name = model.Name;
                //mapAttributes.imei = model.IMEI;
                //mapAttributes.imsi = model.IMSI;
                //mapAttributes.ue_label = model.Ue_Label;
                //mapAttributes.ue_name = model.Ue_Name;
                //mapAttributes.class_id = model.ClassId;
                dbContext.Report_ExcelData.AddRange(dataList);
                dbContext.SaveChanges();

                isSuccess = false;
                //return mapAttributes.attributeid;
            }
            return isSuccess;
        }

        private TabDetailModel getTabDetails()
        {
            TabDetailModel tabDetailModel = new TabDetailModel();

            string tabHeaderHtml = string.Empty;
            string tabContentHtml = string.Empty;
            List<TreeViewNode> tabnodes = null;
            DBHelper db = new DBHelper();
            //List<TreeViewNode> lstIFSData = db.GetIFSData();
            lstIFSData = db.GetIFSData();

            //foreach (TreeViewNode item in lstIFSData.Where(x => x.parentid == "0" && x.text == "PublicFolders"))
            //{
            //    List<TreeViewNode> nodes = getIFSChildNodes(item.id, "/" + item.text);

            //    //treeNode.nodes.Add(node);
            //}

            IFSDataModel node = lstIFSData.FirstOrDefault(x => x.parentid == "0" && x.text == "PublicFolders");
            if (node != null && !string.IsNullOrEmpty(node.id))
            {
                tabnodes = getIFSChildNodes(node.id, "/" + node.text);

                foreach (TreeViewNode item in tabnodes)
                {
                    if (tabnodes.IndexOf(item) == 0)
                    {
                        tabHeaderHtml += @"<li class=""active""><a data-toggle=""tab"" href=""#" + item.text.ToLower() + @""">" + item.text + "</a></li>";
                        tabContentHtml += @"<div id=""" + item.text.ToLower() + @""" class=""tab-pane fade in active"">"
                                        + @"<div id=""treeview" + item.text.ToLower() + @""" class=""treeview""></div>"
                                        + "</div>";
                    }
                    else
                    {
                        tabHeaderHtml += @"<li><a data-toggle=""tab"" href=""#" + item.text.ToLower() + @""">" + item.text + "</a></li>";
                        tabContentHtml += @"<div id=""" + item.text.ToLower() + @""" class=""tab-pane fade"">"
                                        + @"<div id=""treeview" + item.text.ToLower() + @""" class=""treeview""></div>"
                                        + "</div>";
                    }
                }
            }

            //List<TreeViewNode> treeNodelist = new List<TreeViewNode>();
            //treeNodelist.Add(treeNode);

            //string treeViewDatajson = JsonConvert.SerializeObject(treeNodelist);
            //ViewBag.TreeViewData = treeViewDatajson;

            //    tabHeaderHtml = @"<li class=""active""><a data-toggle=""tab"" href=""#geo"">Geo</a></li>"
            //+ @"<li><a data-toggle=""tab"" href=""#report"">Report</a></li>";
            //    tabContentHtml = @"<div id=""geo"" class=""tab-pane fade in active"">"
            //    + "<h3>Geo</h3>"
            //    + "<p>Geo Tree structure</p>"
            //+ "</div>"
            //+ @"<div id=""report"" class=""tab-pane fade"">"
            //    + "<h3>Report</h3>"
            //    + "<p>Report Tree structure</p>"
            //+ "</div>";
            tabDetailModel.tabHeaderHtml = JsonConvert.SerializeObject(tabHeaderHtml);
            tabDetailModel.tabContentHtml = JsonConvert.SerializeObject(tabContentHtml);
            tabDetailModel.tabnodes = JsonConvert.SerializeObject(tabnodes);

            return tabDetailModel;
        }

        private void DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                // If file found, delete it    
                System.IO.File.Delete(filePath);
            }
        }

        private string CalculateGeoReports(ConfigParamModel form, FormCollection formData)
        {
            string reportdata = string.Empty;

            string exePath = ConfigurationManager.AppSettings["GeoExePath"];
            string jsonConfigPath = ConfigurationManager.AppSettings["GeoJsonConfigPath"];
            string tempJsonConfigPath = ConfigurationManager.AppSettings["tempJsonConfigPath"];

            DateTime currdt = DateTime.Now;
            string strTimeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff_");
            string newtempJsonConfigPath = tempJsonConfigPath + strTimeStamp + form.ReportType.ToLower() + "_tmp" + ".json";
            string ttgOutputFilePath = string.Empty;
            string ttgOutputFileName = strTimeStamp + form.ReportName + ".ttg";
            string excelOutputFilePath = string.Empty;
            string excelOutputFileName = strTimeStamp + "report_dashboard.xlsx";
            string excelsheetName = string.Empty;

            try
            {
                //create temp json file with changed values
                string json = System.IO.File.ReadAllText(jsonConfigPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                ttgOutputFilePath = jsonObj["GeoReports"][0]["OutputFile"].ToString() + ttgOutputFileName;
                ////excelOutputFilePath = jsonObj["Export"]["XlsxOutput"]["OutputFile"].ToString() + excelOutputFileName;
                //excelOutputFilePath = jsonObj["XlsxOutput"]["OutputFile"].ToString() + excelOutputFileName;
                //excelsheetName = jsonObj["Reports"][0]["XlsxWorkSheet"].ToString();
                if (form.FromDate != null)
                {
                    jsonObj["GeoReports"][0]["Pixmaps"][0]["TimeFrom"] = form.FromDate.Value.ToString("yyyy-MM-dd 00:00:00");
                }
                if (form.ToDate != null)
                {
                    jsonObj["GeoReports"][0]["Pixmaps"][0]["TimeTo"] = form.ToDate.Value.ToString("yyyy-MM-dd 00:00:00");
                }
                jsonObj["GeoReports"][0]["Pixmaps"][0]["Name"] = form.ReportName;
                jsonObj["GeoReports"][0]["Pixmaps"][0]["PixelSizeMeters"] = form.PixelSizeMeters;
                jsonObj["GeoReports"][0]["Pixmaps"][0]["ObjectAttrFilter"]["EUARFCN"] = form.EUARFCN;
                jsonObj["GeoReports"][0]["Path"] = form.Path;
                jsonObj["GeoReports"][0]["OutputFile"] = ttgOutputFilePath;

                ////jsonObj["Reports"][0]["Objects"] = JsonConvert.SerializeObject(form.PLMNObjects.Split(','));
                //jsonObj["Reports"][0]["Objects"] = Newtonsoft.Json.Linq.JToken.FromObject(form.PLMNObjects.Split(','));

                ////jsonObj["Export"]["XlsxOutput"]["OutputFile"] = excelOutputFilePath;
                //jsonObj["XlsxOutput"]["OutputFile"] = excelOutputFilePath;

                string output = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                System.IO.File.WriteAllText(newtempJsonConfigPath, output);
                //create temp json file with changed values

                Process myProcess = new Process();
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = exePath;
                //myProcess.StartInfo.Arguments = "\"" + jsonConfigPath + "\"";
                myProcess.StartInfo.Arguments = "-c " + newtempJsonConfigPath;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();

                System.Threading.Thread.Sleep(2000);

                DeleteFile(newtempJsonConfigPath);
            }
            catch (Exception ex)
            {
                DeleteFile(newtempJsonConfigPath);
                throw ex;
            }

            //if (!string.IsNullOrEmpty(excelOutputFilePath) && !string.IsNullOrEmpty(excelsheetName))
            //{
            //    //List<Report_ExcelData> dataList = ReadDataFromExcel(excelOutputFilePath, excelsheetName);

            //    //SaveReportExcelData(dataList);

            //    //ViewBag.ReportExcelDatajson = JsonConvert.SerializeObject(dataList);

            //    reportdata = ReadDataFromExcel(excelOutputFilePath, excelsheetName, ttrOutputFilePath);

            //}

            reportdata = ttgOutputFilePath;

            return reportdata;
        }

        private string GetReportTemplate(ConfigParamModel form, FormCollection formData)
        {
            string reportdata = string.Empty;

            DBHelper db = new DBHelper();
            string json = db.GetReportTemplate();

            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            reportdata = "<br/>Name : " + jsonObj["Title"] +
                "<br/>Description : " + jsonObj["Description"];

            return reportdata;
        }
    }
}