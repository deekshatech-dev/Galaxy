using System;
using System.Linq;
using System.Web.Mvc;
using GPSDB;
using GPSMap.Models;

namespace GPSMap.Controllers
{
    public class MultipleController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetBaseMapResultContainer()
        {
            var chart = new KPIChartDataModel();
            using (var dbContext = new DatabaseContext())
            {
                chart.KPIValues = new SelectList(dbContext.kpimaster.Take(15).ToList().Select(n => new SelectListItem
                {
                    Text = n.KPIName,
                    Value = n.KPIId.ToString()
                }), "Value", "Text");
                var chartSearch = new ChartSearchModel();
                chart.SearchModel = chartSearch;
                chart.UniqueId = Guid.NewGuid();
            }

            return View("_MapContainer", chart);
        }
    }
}