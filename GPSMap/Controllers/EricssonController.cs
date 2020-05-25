using GPSDB;
using GPSMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GPSMap.Controllers
{
    [Authorize]
    public class EricssonController : Controller
    {
        // GET: Ericsson
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Ericsson5gXMLSearch form)
        {

            using (var dbContext = new DatabaseContext())
            {
               // var mst = dbContext.ericsson_5gmaster.AsQueryable();
                var dtl = dbContext.ericsson_5gdetail.AsQueryable();
                //var query = (from left in dtl
                //             join right in mst on new { p1 = left.mstid } equals new { p1 = right.mstid } into joinedList
                //             from sub in joinedList.DefaultIfEmpty()
                //             select new ericsson_5gdetail
                //             {
                //                 counter = left.counter,
                //                 ManagedElement = left.ManagedElement,
                //                 MOClass = left.MOClass,
                //                 MOClassValue = left.MOClassValue,
                //                 CounterValue = left.CounterValue,
                //                 CreatedDate = sub.CreatedDate

                //             });

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
            }

            return View(form);
        }

        [HttpPost]
        public FileResult ExportCSV()
        {

            using (var dbContext = new DatabaseContext())
            {
                List<object> ericsson_5gdetail = (from ericsson in dbContext.ericsson_5gdetail.ToList().Take(5000)
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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Ericsson.csv");
            }
        }
    }
}