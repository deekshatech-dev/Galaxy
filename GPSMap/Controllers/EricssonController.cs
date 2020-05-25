using GPSDB;
using GPSMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}