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
using System.Net;
using System.Net.Mail;
using System.IO;

namespace GPSMap.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (this.hasRights("Network Audit"))
            {
                ViewBag.map = "{}";
                ViewBag.KPIValues = KPIValues.LTE_UE_CQI_CQI.ToSelectList();
                var form = new SearchViewModel();
                return View(form);
            }
            else
            {
                return Redirect("/Account/NotAuthorised");
            }
        }

        [HttpPost]
        public ActionResult Index(SearchViewModel form, FormCollection formData)
        {
            if (this.hasRights("Network Audit"))
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
            else
            {
                return Redirect("/Account/NotAuthorised");
            }
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
                    var map = dbContext.ericssonxmldetail.AsQueryable();

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
                    return View("EricssonXMLResult");

                    //return View(form);

                }
            }


        }

        [HttpPost]
        public JsonResult SaveFile(FileContentModel fileContent)
        {
            var fileName = DateTime.Now.Ticks + ".txt";
            if (!System.IO.Directory.Exists(Path.Combine(Server.MapPath("~/EricosnUpload"))))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(Server.MapPath("~/EricosnUpload")));
            }
            var path = Path.Combine(Server.MapPath("~/EricosnUpload/"), fileName);
            var returnValue = new JSONReturnValue();
            try
            {
                using (var file = new System.IO.StreamWriter(path))
                {
                    file.WriteLine(fileContent.Content);
                }
            }
            catch (Exception)
            {
                returnValue.Status = false;
                returnValue.Message = "File Not Saved.";
            }
            try
            {
                var batMasterId = SaveBatMasterAndDetails(fileName, fileContent.Statements);
                if (batMasterId > 0)
                {
                    SendEmail(path, batMasterId);
                    returnValue.Status = true;
                    returnValue.Message = "Mail with file attachment send successfully.";
                }
                else
                {
                    returnValue.Status = false;
                    returnValue.Message = "Record not saved. Please try again";
                }


            }
            catch (Exception)
            {
                returnValue.Status = false;
                returnValue.Message = "Error Sending Mail.";
            }

            return Json(returnValue);
        }

        public bool SendEmail(string fileFullPath, int batMasterId)
        {
            //var fromAddress = new MailAddress("hiteshshah4478@gmail.com", "From Pritesh");
            //var toAddress = new MailAddress("hiteshshah4478@gmail.com", "To Hitesh");
            //const string fromPassword = "Hitesh@123";
            //const string subject = "Approve Bat File";
            //const string body = "Body";

            //using (MailMessage mm = new MailMessage("hiteshshah4478@gmail.com", "hiteshshah4478@gmail.com"))
            //{
            //    mm.Subject = subject;
            //    mm.Body = body;

            //    var path = Path.Combine(Server.MapPath("~/EricosnUpload"), "mockforbat.bat");
            //    mm.Attachments.Add(new Attachment(path));
            //    mm.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = "smtp.gmail.com";
            //    smtp.EnableSsl = true;
            //    smtp.UseDefaultCredentials = false;
            //    NetworkCredential NetworkCred = new NetworkCredential("hiteshshah4478@gmail.com", fromPassword);
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = 587;
            //    smtp.Send(mm);
            //    ViewBag.Message = "Email sent.";
            //}

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("hiteshshah4478@gmail.com");
            msg.To.Add("prashantkanakhara1988@gmail.com");
            msg.Subject = "Bat File Approval";
            var url = "http://localhost:61961/bat/Approve?batMasterId=" + batMasterId;
            msg.Body = string.Format("Hello Mihir <br /><br /> Please approve the attached batch file. Click <a href=\"{0}\" target=\"_blank\">here</a> approve.", url);
            msg.Priority = MailPriority.High;
            msg.IsBodyHtml = true;
            msg.Attachments.Add(new Attachment(fileFullPath));
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("hiteshshah4478@gmail.com", "Hitesh@123");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Port = 587;
            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
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


        public int SaveBatMasterAndDetails(string batFileName, string[] statements)
        {
            var batMasterId = 0;
            using (var dbContext = new DatabaseContext())
            {
                var batMaster = new BatMaster();
                batMaster.CreatedOn = DateTime.Now;
                batMaster.BatCreatedBy = 1;
                batMaster.BatCreatedOn = DateTime.Now;
                batMaster.BatFileName = batFileName;
                dbContext.BatMaster.Add(batMaster);
                dbContext.SaveChanges();
                batMasterId = batMaster.BatMasterId;
                foreach (var statement in statements)
                {
                    var batDetails = new BatDetails();
                    batDetails.CreatedOn = DateTime.Now;
                    batDetails.BatMasterId = batMaster.BatMasterId;
                    batDetails.StatementType = "set";
                    batDetails.Statement = statement;
                    dbContext.BatDetails.Add(batDetails);
                    dbContext.SaveChanges();
                }

            }

            return batMasterId;
        }
    }
}