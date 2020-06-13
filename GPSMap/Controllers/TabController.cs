using GPSMap.Helper;
using GPSMap.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPSMap.Controllers
{
    [Authorize]
    public class TabController : Controller
    {
        // GET: Tab
        public List<TreeViewNode> lstIFSData;
        public ActionResult Index()
        {
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

            TreeViewNode node = lstIFSData.FirstOrDefault(x => x.parentid == "0" && x.text == "PublicFolders");
            if (node != null && !string.IsNullOrEmpty(node.id))
            {
                tabnodes = getIFSChildNodes(node.id, "/" + node.text);

                foreach (TreeViewNode item in tabnodes)
                {
                    if (tabnodes.IndexOf(item) == 0)
                    {
                        tabHeaderHtml += @"<li class=""active""><a data-toggle=""tab"" href=""#" + item.text.ToLower() + @""">" + item.text + "</a></li>";
                        tabContentHtml += @"<div id=""" + item.text.ToLower() + @""" class=""tab-pane fade in active"">"
                                        //+ "<h3>" + item.text + "</h3>"
                                        //+ "<p>" + item.text + " Tree structure</p>"
                                        + @"<div id=""treeview" + item.text.ToLower() + @""" class=""treeview""></div>"
                                        + "</div>";
                    }
                    else
                    {
                        tabHeaderHtml += @"<li><a data-toggle=""tab"" href=""#" + item.text.ToLower() + @""">" + item.text + "</a></li>";
                        tabContentHtml += @"<div id=""" + item.text.ToLower() + @""" class=""tab-pane fade"">"
                                        //+ "<h3>" + item.text + "</h3>"
                                        //+ "<p>" + item.text + " Tree structure</p>"
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

            ViewBag.TabHeaderHtml = JsonConvert.SerializeObject(tabHeaderHtml);
            ViewBag.TabContentHtml = JsonConvert.SerializeObject(tabContentHtml);
            ViewBag.TabNodes = JsonConvert.SerializeObject(tabnodes);
            ViewBag.TreeViewPLMNObjectData = GetPLMNObjectData();

            ConfigParamModel model = new ConfigParamModel();
            model.FromDate = DateTime.Now.AddDays(-7);
            model.ToDate = DateTime.Now;

            return View(model);
        }

        private List<TreeViewNode> getIFSChildNodes(string id, string path)
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
                    node.path = path + "/" + item.text;
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

            foreach (TreeViewNode item in lstIFSData.Where(x => x.parentid == "0"))
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
                foreach (TreeViewNode item in lstIFSData.Where(x => x.parentid == id))
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

        public ActionResult OpenExe()
        {
            return File("D:\\weekly_reports\\tmp.txt", "text/plain");
        }

        [HttpPost]
        public FileResult OpenExe(ConfigParamModel form, FormCollection formData)
        {
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
                jsonObj["Reports"][0]["Path"] = form.Path;
                //jsonObj["Reports"][0]["Objects"] = JsonConvert.SerializeObject(form.PLMNObjects.Split(','));
                //jsonObj["Reports"][0]["Objects"] = form.PLMNObjects.Split(',').ToList();
                jsonObj["Reports"][0]["Objects"] = Newtonsoft.Json.Linq.JToken.FromObject(form.PLMNObjects.Split(','));

                //[ "Fiesta", "Focus", "Mustang" ]
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

    }
}