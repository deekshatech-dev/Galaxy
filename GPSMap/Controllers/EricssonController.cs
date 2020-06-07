using GPSDB;
using GPSMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

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
            }

            return View(chart);
        }

        public List<int> GetDays(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                             // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day).Day)
                             // Map each day to a date
                             .ToList(); // Load dates into a list
        }

        public JsonResult GetChartData(ChartRequest request)
        {
            var KPIValues = new List<ChartKPIValues>();
            using (var dbContext = new DatabaseContext())
            {
                // var kpiName = request.KPIName;
                // var kpiName = "RACH Preamble Response Success Rate";
                // X PLMN KPI = SUM(X KPI Nem, for all the day for all the meneged elements)/ 
                // SUM(Den of X KPI of all the day for all the managed elements) ;

                if (request.KpiId != null && request.KpiId.Length > 0)
                {
                    foreach (var id in request.KpiId)
                    {
                        var records = dbContext.ericsson_kpi_data.Where(k => k.KPIId == id && k.ManagedElement == k.ManagedElement).ToList();
                        var KPI = new ChartKPIValues();
                        var chartValues = new ChartValues();

                        chartValues.ChartData = new List<string>();
                        var date = System.DateTime.Now;
                        if (request.Date != null)
                        {
                            date = Convert.ToDateTime(request.Date);
                        }

                        chartValues.Labels = new List<string>();
                        if (request.Trend == "Monthly")
                        {
                            //var daysOfMonth = this.GetDays(date.Year, date.Month);
                            //var recordquery = from r in records
                            //                  group r by new
                            //                  {
                            //                      r.CreatedDate.Day,
                            //                      r.CreatedDate.Month,
                            //                      r.CreatedDate.Year,
                            //                  } into gcs
                            //                  select new 
                            //                  {
                            //                      Day = gcs.Key.Day,
                            //                      Numerator = gcs.Sum(x=>x.Numerator),
                            //                      Denominator = gcs.Sum(x=>x.Denominator)
                            //                  };

                            //var dsd = recordquery.ToList();
                            //var finallist = (from d in daysOfMonth
                            //                join rq in recordquery on d equals rq.Day into gj
                            //                from days in gj.DefaultIfEmpty()
                            //                select new {
                            //                    Day = d,
                            //                    KPIValue = days?.Denominator != null && days?.Denominator > 0 ? days?.Numerator / days?.Denominator : days?.Numerator
                            //                }).ToList();
                            //var dd = finallist;
                            foreach (var item in this.GetDays(date.Year, date.Month))
                            {

                                if (!string.IsNullOrEmpty(Convert.ToString(item)))
                                {
                                    var dayRecord = records.Where(g => g.CreatedDate.Day == item && g.CreatedDate.Month == date.Month);
                                    var numeratorSum = 0M;
                                    var denominatorSum = 0M;
                                    var dayKPIValue = numeratorSum;
                                    if (dayRecord.Any())
                                    {
                                        numeratorSum = dayRecord.Sum(s => s.Numerator);
                                        denominatorSum = dayRecord.Sum(s => s.Denominator);
                                        if (denominatorSum > 0)
                                        {
                                            dayKPIValue = numeratorSum / denominatorSum;
                                        }
                                    }

                                    chartValues.ChartData.Add(dayKPIValue.ToString());
                                    chartValues.Labels.Add(string.Format("{0}/{1}/{2}", date.Year, date.Month.ToString("00"),item.ToString("00")));
                                }
                            }
                        }
                        else
                        {
                            // chartValues.Labels = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.ToList();
                            for (var i = 0; i <= 23; i++)
                            {
                                // var dayValue = records.Where(g => DateTime.ParseExact(g.CreatedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).Month == i).Sum(s => s.KPIValue.ToDecimal());
                                var hourRecord = records.Where(g => g.CreatedDate.Hour == i);
                                var numeratorSum = 0M;
                                var denominatorSum = 0M;
                                var hourKPIValue = numeratorSum;
                                if (hourRecord.Any())
                                {
                                    numeratorSum = hourRecord.Sum(s => s.Numerator);
                                    denominatorSum = hourRecord.Sum(s => s.Denominator);
                                    if (denominatorSum > 0)
                                    {
                                        hourKPIValue = numeratorSum / denominatorSum;
                                    }
                                }
                                chartValues.Labels.Add(i.ToString());
                                chartValues.ChartData.Add(hourKPIValue.ToString());
                            }
                        }


                        KPI.KPI = dbContext.kpimaster.FirstOrDefault(s=>s.KPIId == id).KPIName;
                        KPI.ChartData = chartValues;
                        KPIValues.Add(KPI);
                    }
                }
            } 
            return Json(KPIValues, JsonRequestBehavior.AllowGet);
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