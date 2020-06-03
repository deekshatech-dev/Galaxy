using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Configuration;
using GPSMap.Models;

namespace GPSMap.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult OpenExe()
        //{
        //    return File("D:\\weekly_reports\\tmp.txt", "text/plain");
        //}

        [HttpPost]
        //public ActionResult OpenExe(SearchViewModel form, FormCollection formData)
        public FileResult OpenExe(SearchViewModel form, FormCollection formData)
        {
            //openexe();
            string exePath = ConfigurationManager.AppSettings["ExePath"];
            string jsonConfigPath = ConfigurationManager.AppSettings["JsonConfigPath"];

            try
            {
                //create json file
                //change values in file

                Process myProcess = new Process();
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = exePath;
                //myProcess.StartInfo.Arguments = "\"" + jsonConfigPath + "\"";
                myProcess.StartInfo.Arguments = "-c " + jsonConfigPath;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();

                //delete file
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return View();
            //return null;

            //return new EmptyResult();
            //return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Ericsson.csv");
            //return File(@"D:\weekly_reports\$2020_$06_$03_Cluster daily.ttr", "text/plain");
            //return File(@"D:\weekly_reports\2020_05_26_Cluster daily.ttr", "text/plain", "ClusterReport.ttr");
            return File(@"D:\weekly_reports\2020_05_26_Cluster daily.ttr", "text/plain", "ClusterReport.ttr");
            //return File("D:\\weekly_reports\\tmp.txt", "text/plain");

            //return new HttpStatusCodeResult(204);

        }
    }
}