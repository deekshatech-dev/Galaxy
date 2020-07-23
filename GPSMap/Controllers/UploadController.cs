using GPSMap.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using GPSDB;
using System.Web;
using System.IO;
using NMFReaderForGPS;
using GPSMap.Parser;
using GPSMap.Models;

namespace GPSMap.Controllers
{
    [Authorize]
    public class UploadController : BaseController
    {
        // GET: Upload
        public ActionResult Index()
        {
            if (base.hasRights("Network Audit"))
            {
                return View();
            }
            else
            {
                return Redirect("/Account/NotAuthorised");
            }
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var allowedExtensions = new[] { ".nmf" };
                var extension = Path.GetExtension(file.FileName);
                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.UploadStatus = "InvalidFileType";
                    return View();
                }

                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);

                if (!System.IO.Directory.Exists(Path.Combine(Server.MapPath("~/App_Data/uploads"))))
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(Server.MapPath("~/App_Data/uploads")));
                }

                if (!System.IO.File.Exists(path))
                {
                    file.SaveAs(path);
                }

                var nmfParser = new NMFParser();
                nmfParser.filePath = path;
                var attribute = nmfParser.ParseAttributes();
                var attributeId = SaveAttributes(attribute);
                Program.ReadNMLFile(path, attributeId);
                ViewBag.UploadStatus = "Success";
            }
            else
            {
                ViewBag.UploadStatus = "Failed";
            }

            return View();
        }

        private int SaveAttributes(MapAttributesModel model)
        {
            using (var dbContext = new DatabaseContext())
            {
                var mapAttributes = new MapAttributes();
                mapAttributes.name = model.Name;
                mapAttributes.imei = model.IMEI;
                mapAttributes.imsi = model.IMSI;
                mapAttributes.ue_label = model.Ue_Label;
                mapAttributes.ue_name = model.Ue_Name;
                mapAttributes.class_id = model.ClassId;
                dbContext.MapAttributes.Add(mapAttributes);
                dbContext.SaveChanges();

                return mapAttributes.attributeid;
            }
        }
    }
}

