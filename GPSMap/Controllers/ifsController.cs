using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Configuration;
using GPSMap.Models;
using System.IO;
using Newtonsoft.Json;
using GPSMap.Helper;

namespace GPSMap.Controllers
{
    [Authorize]
    public class ifsController : Controller
    {
        // GET: ifs
        public List<TreeViewNode> lstIFSData;
        public ActionResult Index()
        {
            //openexe();
            //select id, parent_id, name from tt_ifs
            //lte_ericsson_12282019

            TreeViewNode treeNode = new TreeViewNode();
            treeNode.text = "Public Directories";
            treeNode.href = "#pd";
            treeNode.tags = new List<string>() { "10" };

            //TreeViewNode geoNode = new TreeViewNode();
            //geoNode.text = "GEO";
            //geoNode.href = "#geo";
            //geoNode.tags = new List<string>() { "4" };

            //TreeViewNode airintrfaceNode = new TreeViewNode();
            //airintrfaceNode.text = "Air Interface";
            //airintrfaceNode.href = "#interface";
            //airintrfaceNode.tags = new List<string>() { "0" };

            //TreeViewNode servicesNode = new TreeViewNode();
            //servicesNode.text = "Quality of Services";
            //servicesNode.href = "#services";
            //servicesNode.tags = new List<string>() { "0" };

            //geoNode.nodes = new List<TreeViewNode>();
            //geoNode.nodes.Add(airintrfaceNode);
            //geoNode.nodes.Add(servicesNode);

            //TreeViewNode reportNode = new TreeViewNode();
            //reportNode.text = "Report";
            //reportNode.href = "#report";
            //reportNode.tags = new List<string>() { "4" };

            //TreeViewNode level1Node = new TreeViewNode();
            //level1Node.text = "Level1";
            //level1Node.href = "#indicators";
            //level1Node.tags = new List<string>() { "0" };

            //TreeViewNode level2Node = new TreeViewNode();
            //level2Node.text = "Level2";
            //level2Node.href = "#analysis";
            //level2Node.tags = new List<string>() { "0" };

            //reportNode.nodes = new List<TreeViewNode>();
            //reportNode.nodes.Add(level1Node);
            //reportNode.nodes.Add(level2Node);

            treeNode.nodes = new List<TreeViewNode>();
            //treeNode.nodes.Add(geoNode);
            //treeNode.nodes.Add(reportNode);

            DBHelper db = new DBHelper();
            //List<TreeViewNode> lstIFSData = db.GetIFSData();
            lstIFSData = db.GetIFSData();

            foreach (TreeViewNode item in lstIFSData.Where(x => x.parentid == "0" && x.text == "PublicFolders"))
            {
                TreeViewNode node = new TreeViewNode();
                node.text = item.text;
                node.href = "#" + item.text;
                node.isDirectory = item.isDirectory;
                node.tags = new List<string>() { "0" };

                //node.nodes = new List<TreeViewNode>();
                //node.nodes.Add(airintrfaceNode);
                //node.nodes.Add(servicesNode);

                node.nodes = getChildNodes(item.id);

                treeNode.nodes.Add(node);
            }

            List<TreeViewNode> treeNodelist = new List<TreeViewNode>();
            treeNodelist.Add(treeNode);

            string treeViewDatajson = JsonConvert.SerializeObject(treeNodelist);
            ViewBag.TreeViewData = treeViewDatajson;

            ViewBag.TreeViewPLMNObjectData = GetPLMNObjectData();

            return View();
        }

        private List<TreeViewNode> getChildNodes(string id)
        {
            List<TreeViewNode> childnodes = null;

            if (lstIFSData.Any(x => x.parentid == id))
            {
                childnodes = new List<TreeViewNode>();
                foreach (TreeViewNode item in lstIFSData.Where(x => x.parentid == id))
                {
                    TreeViewNode node = new TreeViewNode();
                    node.text = item.text;
                    node.href = "#" + item.text;
                    node.isDirectory = item.isDirectory;
                    node.tags = new List<string>() { "0" };

                    //node.nodes = new List<TreeViewNode>();
                    //node.nodes.Add(airintrfaceNode);
                    //node.nodes.Add(servicesNode);

                    node.nodes = getChildNodes(item.id);

                    childnodes.Add(node);
                }
            }

            return childnodes;
        }

        public ActionResult OpenExe()
        {
            return File("D:\\weekly_reports\\tmp.txt", "text/plain");
        }

        [HttpPost]
        //public ActionResult OpenExe(SearchViewModel form, FormCollection formData)
        public FileResult OpenExe(SearchViewModel form, FormCollection formData)
        {
            //openexe();
            string exePath = ConfigurationManager.AppSettings["ExePath"];
            string jsonConfigPath = ConfigurationManager.AppSettings["JsonConfigPath"];
            string tempJsonConfigPath = ConfigurationManager.AppSettings["tempJsonConfigPath"];

            DateTime currdt = DateTime.Now;
            string newtempJsonConfigPath = tempJsonConfigPath + "tmp" + currdt.ToString("_MMddyyyy_HHmmss") + ".json";

            try
            {
                //create temp json file with changed values
                string json = System.IO.File.ReadAllText(jsonConfigPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                if (form.FromDate != null)
                {
                    jsonObj["Reports"][0]["TimeFrom"] = form.FromDate.Value.Subtract(currdt).Days.ToString();
                }
                if (form.ToDate != null)
                {
                    jsonObj["Reports"][0]["TimeTo"] = form.ToDate.Value.Subtract(currdt).Days.ToString();
                }
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

                //if file exists then delete file
                if (System.IO.File.Exists(newtempJsonConfigPath))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(newtempJsonConfigPath);
                }
            }
            catch (Exception ex)
            {
                //if file exists then delete file
                if (System.IO.File.Exists(newtempJsonConfigPath))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(newtempJsonConfigPath);
                }
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

            //if (!CanEnterAction(nameof(MyAction))) return new HttpStatusCodeResult(204);
            //try
            //{
            //    // Do long running stuff
            //    //return ValidActionResult();
            //}
            //finally
            //{
            //    ExitedAction(nameof(MyAction));
            //}
        }
        //public void openexe()
        //{

        //}

        private string GetPLMNObjectData()
        {

            DBHelper db = new DBHelper();
            //List<TreeViewNode> lstIFSData = db.GetIFSData();
            lstIFSData = db.GetPLMNObjectData();

            List<TreeViewNode> treeNodelist = new List<TreeViewNode>();

            foreach (TreeViewNode item in lstIFSData.Where(x => x.parentid == "0"))
            {
                TreeViewNode node = new TreeViewNode();
                node.text = item.text;
                node.href = "#" + item.text;
                node.isDirectory = item.isDirectory;
                node.tags = new List<string>() { "0" };

                //node.nodes = new List<TreeViewNode>();
                //node.nodes.Add(airintrfaceNode);
                //node.nodes.Add(servicesNode);

                node.nodes = getChildNodes(item.id);

                //treeNode.nodes.Add(node);
                treeNodelist.Add(node);
            }

            //List<TreeViewNode> treeNodelist = new List<TreeViewNode>();
            //treeNodelist.Add(treeNode);

            string treeViewDatajson = JsonConvert.SerializeObject(treeNodelist);
            return treeViewDatajson;
        }
    }

}