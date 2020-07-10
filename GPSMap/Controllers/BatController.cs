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
    [AllowAnonymous]
    public class BatController : Controller
    {
        // GET: Bat
        public ActionResult Approve(int batMasterId)
        {
            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    var batMaster = dbContext.BatMaster.FirstOrDefault(b => b.BatMasterId == batMasterId);
                    batMaster.BatApprovedOn = DateTime.Now;
                    // batMaster.BatApprovedBy = 0;
                    dbContext.SaveChanges();
                    ViewBag.Status = "Approved";
                }
                catch (Exception ex)
                {
                    ViewBag.Status = "Failed";
                }

                return View("~/Views/Account/Login.cshtml");
            }
        }
    }
}