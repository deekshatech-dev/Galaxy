using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GPSDB;
using System.Web;
using System.IO;
using GPSMap.Models;
using System.Xml;
using System.Data;
using OfficeOpenXml;
using GPSMap.Data;

namespace GPSMap.Controllers
{
    public class EricssonUploadController : BaseController
    {
        // GET: EricssonUpload
        public ActionResult Index()
        {
            if (this.hasRights("PM Counter"))
            {
                return View();
            }
            else
            {
                return Redirect("/Account/NotAuthorised");
            }
        }


        [HttpPost]
        public ActionResult Index(HttpPostedFileBase[] file)
        {
            List<string> xmlfilename = new List<string>();
            string excelfilename = "";
            foreach (HttpPostedFileBase ff in file)
            {
                if (ff.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".xml", ".xlsx", ".xlsm" };
                    var extension = Path.GetExtension(ff.FileName);
                    if (!allowedExtensions.Contains(extension))
                    {
                        ViewBag.UploadStatus = "InvalidFileType";
                        return View();
                    }

                    var fileName = Path.GetFileName(ff.FileName);
                    var path = Path.Combine(Server.MapPath("~/EricosnUpload"), fileName);

                    if (!System.IO.Directory.Exists(Path.Combine(Server.MapPath("~/EricosnUpload"))))
                    {
                        System.IO.Directory.CreateDirectory(Path.Combine(Server.MapPath("~/EricosnUpload")));
                    }

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    if (!System.IO.File.Exists(path))
                    {
                        ff.SaveAs(path);
                        if (extension == ".xml")
                        {
                            xmlfilename.Add(path);

                        }
                        else
                        {
                            excelfilename = path;
                        }
                    }


                    ViewBag.UploadStatus = "Success";
                }
                else
                {
                    ViewBag.UploadStatus = "Failed";
                }

            }


            foreach (string abcfilename in xmlfilename)
            {
                ReadXMLEricsson(abcfilename);
            }

            return View();
        }


        private void ReadXMLEricsson(string FN)
        {
            try
            {

                var path = Path.Combine(Server.MapPath("~/EricosnUpload"), "Parameters.xlsx");


                DataTable data = ReadDataFromParameterExcel(path, "EricssonGVP");

                var ImpValue = data.AsEnumerable()
              .Select(dataRow => new EricssonGV
              {
                  MOC = dataRow.Field<string>("MOC"),
                  PN = dataRow.Field<string>("Parameter_Name")

              }).ToList();





                XmlDocument doc = new XmlDocument();
                xmlDataRawEricsson XMLEricsson = new xmlDataRawEricsson();
                XMLEricsson.xmlDataEricsson = null;
                XMLEricsson.xmlDataEricsson = new List<ericssonxmldetail>();

                doc.Load(FN);
                XmlNodeList SubNetWorkList = doc.GetElementsByTagName("xn:SubNetwork");

                foreach (XmlNode objSubNetWork in SubNetWorkList)
                {
                    if (objSubNetWork.ParentNode.Name != "xn:SubNetwork")
                    {

                    }
                    else
                    {

                        string subnetwork = objSubNetWork.Attributes["id"].InnerText;

                        SubNetWorkList = null;
                        XmlDocument docManagedElement = new XmlDocument();
                        docManagedElement.LoadXml(objSubNetWork.InnerXml);

                        XmlNodeList MElement = docManagedElement.GetElementsByTagName("xn:ManagedElement");
                        string ManagedElement = MElement[0].Attributes["id"].InnerText;
                        string MEContext = "";
                        foreach (XmlNode nodeME in MElement[0].ChildNodes)
                        {

                            if (nodeME.Name == "xn:attributes")
                            {
                                MEContext = nodeME.FirstChild.InnerText;
                            }
                            else if (nodeME.Name == "xn:VsDataContainer")
                            {
                                ///Code
                                readdatacontainernode(nodeME, ImpValue, FN, subnetwork, MEContext, ref XMLEricsson);

                                ///Code End



                            }

                        }


                    }
                }






                //var queryT = (from left in XMLEricsson.xmlDataEricsson
                //              join right in ImpValue on new { p1 = left.Paramenter.ToString().Trim().ToUpper() } equals new { p1 = right.PN.ToString().Trim().ToUpper() } into joinedList
                //              from sub in joinedList.DefaultIfEmpty()
                //              select new listItemEricsson
                //              {
                //                  SubNetwork = left.SubNetwork,
                //                  ManagedElement = left.ManagedElement,
                //                  Cell = left.Cell,
                //                  Paramenter = left.Paramenter,
                //                  Schedule = left.Schedule,
                //                  Discrepancy = "",
                //                  GUIValue = sub == null ? "NULL" : "",
                //                  Value = left.Value

                //              }).ToList();


                //var queryT2 = queryT.Where(x => x.GUIValue != "NULL").Select(m => new listItemEricsson
                //{

                //    SubNetwork = m.SubNetwork,
                //    ManagedElement = m.ManagedElement,
                //    Cell = m.Cell,
                //    Paramenter = m.Paramenter,
                //    Schedule = m.Schedule,
                //    Discrepancy = ReturnDesp(m.Value, m.GUIValue),
                //    GUIValue = m.GUIValue,
                //    Value = m.Value

                //}).ToList();






            }
            catch (Exception ex)
            {

            }
        }

        private void readdatacontainernode(XmlNode nodeME, List<EricssonGV> ImpValue, string FN, string subnetwork, string MEContext, ref xmlDataRawEricsson xmlfiledata)
        {
            try
            {
                string Cell = nodeME.Attributes["id"].InnerText;
                int countmoc = 0;
                string tempmoc = "";
                string originalmoc = "";
                foreach (XmlNode parameterlist1 in nodeME.ChildNodes)
                {

                    if (parameterlist1.Name.Contains("attributes"))
                    {
                        foreach (XmlNode parameterlist in parameterlist1.ChildNodes)
                        {
                            if (parameterlist.Name == "xn:vsDataFormatVersion")
                            {
                                continue;
                            }
                            else if (parameterlist.Name == "xn:vsDataType")
                            {
                                originalmoc = tempmoc = parameterlist.FirstChild.Value;
                                tempmoc = tempmoc.Replace("vsData", "").ToUpper().Trim();

                                countmoc = ImpValue.Where(x => x.MOC.ToUpper().Trim().ToString() == tempmoc).Count();

                                continue;

                            }
                            if (parameterlist.Name.Contains(originalmoc))
                            {
                                if (countmoc == 0)
                                {
                                    continue;
                                }


                                foreach (XmlNode parameter in parameterlist.ChildNodes)
                                {
                                    if (!parameter.HasChildNodes)
                                    {

                                        string parametername = parameter.Name.Contains(':') == true ? parameter.Name.Split(':')[1].ToString() : parameter.Name;
                                        if (ImpValue.Where(x => x.PN.ToUpper().Trim().ToString() == parametername.ToUpper().Trim()).Count() == 0)
                                        {
                                            continue;
                                        }

                                        ericssonxmldetail obj;
                                        if (parametername == "UeMeasControlID")
                                        {

                                            obj = new ericssonxmldetail
                                            {

                                                SubNetwork = subnetwork,
                                                DataContainer = Cell,
                                                MeContext = MEContext,
                                                UeMeasControlId = parameter.InnerText,
                                                MOC = tempmoc,
                                                Parameter = parametername,
                                                Value = parameter.InnerText,
                                                Discrepency = "",
                                                GUIValue = "",
                                                Schedule = ""
                                            };

                                        }
                                        else
                                        {
                                            obj = new ericssonxmldetail
                                            {

                                                SubNetwork = subnetwork,
                                                DataContainer = Cell,
                                                MeContext = MEContext,
                                                UeMeasControlId = Cell,
                                                MOC = tempmoc,
                                                Parameter = parametername,
                                                Value = parameter.InnerText,
                                                Discrepency = "",
                                                GUIValue = "",
                                                Schedule = ""
                                            };
                                        }
                                        xmlfiledata.xmlDataEricsson.Add(obj);

                                    }
                                    else
                                    {

                                        foreach (XmlNode parameterchildnode in parameter.ChildNodes)
                                        {
                                            string parametername = parameter.Name.Contains(':') == true ? parameter.Name.Split(':')[1].ToString() : parameter.Name;
                                            if (ImpValue.Where(x => x.PN.ToUpper().Trim().ToString() == parametername.ToUpper().Trim()).Count() == 0)
                                            {
                                                continue;
                                            }

                                            ericssonxmldetail obj;
                                            if (parametername == "UeMeasControlID")
                                            {

                                                obj = new ericssonxmldetail
                                                {
                                                    SubNetwork = subnetwork,
                                                    DataContainer = Cell,
                                                    MeContext = MEContext,
                                                    UeMeasControlId = parameter.InnerText,
                                                    MOC = tempmoc,
                                                    Parameter = parametername,
                                                    Value = parameter.InnerText,
                                                    Discrepency = "",
                                                    GUIValue = "",
                                                    Schedule = ""
                                                };

                                            }
                                            else
                                            {
                                                obj = new ericssonxmldetail
                                                {

                                                    SubNetwork = subnetwork,
                                                    DataContainer = Cell,
                                                    MeContext = MEContext,
                                                    UeMeasControlId = Cell,
                                                    MOC = tempmoc,
                                                    Parameter = parametername,
                                                    Value = parameter.InnerText,
                                                    Discrepency = "",
                                                    GUIValue = "",
                                                    Schedule = ""
                                                };
                                            }
                                            xmlfiledata.xmlDataEricsson.Add(obj);
                                        }

                                    }
                                }

                            }
                        }
                    }
                    else if (parameterlist1.Name.Contains("VsDataContainer"))
                    {
                        readdatacontainernode(parameterlist1, ImpValue, FN, subnetwork, MEContext, ref xmlfiledata);

                    }

                }
            }
            catch (Exception ex)
            {
                //  errorlogger.add("Exception from readdatacontainernode : " + ex.Message.ToString() + "\n");

            }

        }


        private DataTable ReadDataFromParameterExcel(string excelOutputFilePath, string excelsheetName)
        {
            //var dataList = new List<ReportExcelColumnModel>();
            //List<Report_ExcelData> dataList = new List<Report_ExcelData>();
            DataTable reportExcelData = null;

            bool isCatchFileAccesException = true;
            while (isCatchFileAccesException)
            {
                try
                {


                    using (var stream = System.IO.File.Open(excelOutputFilePath, FileMode.Open, FileAccess.Read))
                    {
                        isCatchFileAccesException = false;
                        //using (var package = new ExcelPackage(file.InputStream))
                        // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(stream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            //var workSheet = currentSheet.First();
                            var workSheet = currentSheet[excelsheetName];
                            ////////
                            //DataTable dt = workSheet.ToDataTable();
                            reportExcelData = workSheet.ToDataTablePara();

                        }
                    }


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

    }
}