using GPSDB;
using GPSMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GPSMap.Helper;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using GPSMap.Data;

namespace GPSMap.Controllers
{


    [Authorize]
    public class EricssonController : Controller
    {
        public List<GPSDB.ericsson_5gdetail> p_ericsson_5gdetail { get; set; }
        // GET: Ericsson
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Index")]
        public ActionResult Index(Ericsson5gXMLSearch form)
        {

            using (var dbContext = new DatabaseContext())
            {
                // var mst = dbContext.ericsson_5gmaster.AsQueryable();
                var dtl = dbContext.ericsson_5gdetail.AsQueryable();

                if (form.ToDate != null)
                {
                    dtl = dtl.Where(t => t.CreatedDate <= form.ToDate.Value);
                }
                if (form.FromDate != null)
                {
                    dtl = dtl.Where(t => t.CreatedDate >= form.FromDate.Value);
                }
                if (form.MOClass != null)
                {
                    dtl = dtl.Where(t => t.MOClass.Contains(form.MOClass));
                }
                if (form.ManagedElement != null)
                {
                    dtl = dtl.Where(t => t.ManagedElement.Contains(form.ManagedElement));
                }
                if (form.counter != null)
                {
                    dtl = dtl.Where(t => t.counter.Contains(form.counter));
                }
                var filterd = dtl.ToList().Take(5000);

                ViewBag.map = filterd;
                p_ericsson_5gdetail = dtl.ToList();


            }

            return View(form);
        }

        public ActionResult Dashboard()
        {
            ViewBag.map = "{}";
            using (var dbContext = new DatabaseContext())
            {
                ViewBag.KPIValues = new SelectList(dbContext.ericsson_kpi_5g_data.Distinct().Take(15).ToList().Select(n => new SelectListItem
                {
                    Text = n.KPIName,
                    Value = n.KPIValue
                }), "Value", "Text");
            }
            var form = new SearchViewModel();
            return View(form);
        }

        [HttpPost]
        public ActionResult Dashboard(SearchViewModel form, FormCollection formData)
        {
            using (var dbContext = new DatabaseContext())
            {
                var map = dbContext.MapPoints.Join(dbContext.MapAttributes,
                     mp => mp.attribute_id,
                     ma => ma.attributeid,
                    (mp, ma) => new { MapPoints = mp, MapAttributes = ma }).AsQueryable();

                if (form.ToDate != null)
                {
                    map = map.Where(t => t.MapPoints.period_to <= form.ToDate.Value);
                }
                if (form.FromDate != null)
                {
                    map = map.Where(t => t.MapPoints.period_from >= form.FromDate.Value);
                }
                if (form.Id != null && form.Id.Any())
                {
                    map = map.Where(t => form.Id.Contains(t.MapPoints.Id));
                }

                var maplist = map.ToList();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                ViewBag.map = serializer.Serialize(maplist);
            }

            ViewBag.KPIValues = KPIValues.LTE_UE_CQI_CQI.ToSelectList(formData["KPI"]);
            form.KPI = formData["KPI"];
            return View(form);
        }



        [HttpPost]
        [MultipleButton(Name = "action", Argument = "ExportCSV")]
        public FileResult ExportCSV(Ericsson5gXMLSearch form)
        {


            using (var dbContext = new DatabaseContext())
            {
                var dtl = dbContext.ericsson_5gdetail.AsQueryable();

                if (form.ToDate != null)
                {
                    dtl = dtl.Where(t => t.CreatedDate <= form.ToDate.Value);
                }
                if (form.FromDate != null)
                {
                    dtl = dtl.Where(t => t.CreatedDate >= form.FromDate.Value);
                }
                if (form.MOClass != null)
                {
                    dtl = dtl.Where(t => t.MOClass.Contains(form.MOClass));
                }
                if (form.ManagedElement != null)
                {
                    dtl = dtl.Where(t => t.ManagedElement.Contains(form.ManagedElement));
                }
                if (form.counter != null)
                {
                    dtl = dtl.Where(t => t.counter.Contains(form.counter));
                }


                List<object> ericsson_5gdetail = (from ericsson in dtl.ToList().Take(5000)
                                                  select new[] { ericsson.MOClass.ToString(),
                                                                   ericsson.MOClassValue,
                                                                  ericsson.JobID,
                                                                  ericsson.ManagedElement,
                                                                  ericsson.Equipment,
                                                                  ericsson.MOClass1,
                                                                  ericsson.MOClass2,
                                                                  ericsson.counter,
                                                                  ericsson.CellName,
                                                                  ericsson.CounterValue,
                                                      }).ToList<object>();

                ericsson_5gdetail.Insert(0, new string[10] { "MOClass", "MOClassValue", "JobID", "ManagedElement", "Equipment", "MOClass1", "MOClass2", "counter", "CellName", "CounterValue" });

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ericsson_5gdetail.Count; i++)
                {

                    string[] ericsson_dtl = (string[])ericsson_5gdetail[i];
                    for (int j = 0; j < ericsson_dtl.Length; j++)
                    {
                        //Append data with separator.
                        sb.Append(ericsson_dtl[j] + ',');
                    }

                    //Append new line character.
                    sb.Append("\r\n");

                }
                Ericsson5gXMLSearch ercs = new Ericsson5gXMLSearch();

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Ericsson.csv");

            }




        }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultipleButtonAttribute : ActionNameSelectorAttribute
    {
        public string Name { get; set; }
        public string Argument { get; set; }
        public object ViewBag { get; private set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var isValidName = false;
            var keyValue = string.Format("{0}:{1}", Name, Argument);
            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

            if (value != null)
            {
                controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                isValidName = true;
            }

            return isValidName;
        }        
    }

}