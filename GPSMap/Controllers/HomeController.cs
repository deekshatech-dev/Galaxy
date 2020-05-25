using GPSMap.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using GPSDB;
using GPSMap.Models;
using System.Web.Script.Serialization;
using GPSMap.Data;

namespace GPSMap.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.map = "{}";
            ViewBag.KPIValues = KPIValues.LTE_UE_CQI_CQI.ToSelectList();
            var form = new SearchViewModel();
            return View(form);
        }

        [HttpPost]
        public ActionResult Index(SearchViewModel form, FormCollection formData)
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

        public ActionResult GPSMap()
        {
            string csvFile = @"D:\mapplot\GPSCoordinate_2020-04-27.csv";
            List<CoOrdinateInfo> values = System.IO.File.ReadAllLines(csvFile)
                                          .Skip(1)
                                          .Select(v => FromCsv(v))
                                          .ToList();

            var json = JsonConvert.SerializeObject(values.ToList());



            ViewBag.Markers = json.ToString();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";


            DBHelper db = new DBHelper();
            List<LocationDTO> lstLocation = new List<LocationDTO>();

            lstLocation = db.GetLocationData();
            LocationDTO loc = lstLocation.First();
            ViewBag.ListOfLocation = lstLocation;

            return View();
        }


        public ActionResult CQIView()
        {
            DBHelper db = new DBHelper();
            List<CQIData> lstCQIData = new List<CQIData>();
            lstCQIData = db.GetCQIData();
            var json = JsonConvert.SerializeObject(lstCQIData.ToList());
            ViewBag.Markers = json.ToString();
            return View();
        }



        public ActionResult RSRPView()
        {
            DBHelper db = new DBHelper();
            List<RSRPData> lstRSRPData = new List<RSRPData>();
            lstRSRPData = db.GetRSRPData();
            var json = JsonConvert.SerializeObject(lstRSRPData.ToList());
            ViewBag.Markers = json.ToString();
            return View();
        }

        public ActionResult RSSNRView()
        {
            DBHelper db = new DBHelper();
            List<RSSNRData> lstRSSNRData = new List<RSSNRData>();
            lstRSSNRData = db.GetRSSNRData();
            var json = JsonConvert.SerializeObject(lstRSSNRData.ToList());
            ViewBag.Markers = json.ToString();
            return View();
        }


        public JsonResult GetCoordinate()
        {
            string csvFile = @"D:\pritesh\GPSPloting\GPSMap\GPSMap\CSVOutPut\GPSCoordinate_2020-04-23-14-37-48.csv";
            List<CoOrdinateInfo> values = System.IO.File.ReadAllLines(csvFile)
                                          .Skip(1)
                                          .Select(v => FromCsv(v))
                                          .ToList();

            var json = JsonConvert.SerializeObject(values);

            return Json(json, JsonRequestBehavior.AllowGet);
        }


        public static CoOrdinateInfo FromCsv(string csvLine)
        {
            CoOrdinateInfo objLoc = new CoOrdinateInfo();

            string[] values = csvLine.Split(',');

            objLoc.sLongitude = Convert.ToString(values[1]);
            objLoc.sLatitude = Convert.ToString(values[2]);
            objLoc.sCQI = Convert.ToString(values[4]);
            return objLoc;
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult XMLResult()
        {
            ViewBag.map = null;
            return View();
        }
        [HttpPost]
        public ActionResult XMLResult(SearchXMLViewModel form)
        {
            using (var dbContext = new DatabaseContext())
            {
                if (form.VendorType == "Nokia")
                {
                    var map = dbContext.nokiaxmldetail.AsQueryable();

                    if (form.ToDate != null)
                    {
                        map = map.Where(t => t.CreatedDate <= form.ToDate.Value);
                    }
                    if (form.FromDate != null)
                    {
                        map = map.Where(t => t.CreatedDate >= form.FromDate.Value);
                    }

                    var maplist = map.ToList().Take(5000);

                    ViewBag.map = maplist;
                    return View(form);
                }
                else
                {
                    //var map = dbContext.ericssonxmldetail.AsQueryable();

                    //if (form.ToDate != null)
                    //{
                    //    map = map.Where(t => t.CreatedDate <= form.ToDate.Value);
                    //}
                    //if (form.FromDate != null)
                    //{
                    //    map = map.Where(t => t.CreatedDate >= form.FromDate.Value);
                    //}

                    //var maplist = map.ToList().Take(5000);

                    //ViewBag.map = maplist;
                    //return View("EricssonXMLResult");

                    return View(form);

                }
            }

           
        }


        protected override JsonResult Json(object data,
           string contentType, Encoding contentEncoding,
           JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}