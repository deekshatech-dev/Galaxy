using GPSDB;
using GPSMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GPSMap.Helper;
using System.Web.Security;

namespace GPSMap.Controllers
{
    public class BaseController : Controller
    {
        public BaseController() {
            //string action = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            //var currentUser = Membership.GetUser();
            //using (var dbContext = new DatabaseContext())
            //{
            //    var log = new Log();
            //    log.UserId = Convert.ToInt32(currentUser.ProviderUserKey.ToString());
            //    log.Controller = controller;
            //    log.Action = action;
            //    log.Type = "";
            //    log.CreatedOn = System.DateTime.Now;
            //    dbContext.Log.Add(log);
            //    dbContext.SaveChanges();
            //}
        }

        public void Logged() {
            //string action = this.GetType().Name;
            //string s = this.HttpContext.Request.HttpMethod;
        }
    }
}