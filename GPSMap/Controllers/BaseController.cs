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
        public BaseController()
        {
            //string action = this.ControllerContext.RouteData.Values["action"].ToString();
            //string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            // var currentUser = Membership.GetUser();
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

            using (var dbContext = new DatabaseContext())
            {
                string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
                string controller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
                string type = System.Web.HttpContext.Current.Request.HttpMethod;
                var userName = System.Web.HttpContext.Current.User.Identity.Name;
                var user = dbContext.Users.FirstOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    var log = new Log();
                    log.UserId = user.UserId;
                    log.Controller = controller;
                    log.Action = action;
                    log.Type = type;
                    log.CreatedOn = System.DateTime.Now;
                    dbContext.Log.Add(log);
                    dbContext.SaveChanges();
                }
            }
        }
        public List<Rights> GetRights(string Username)
        {
            var returnValue = new List<Rights>();
            using (var dbContext = new DatabaseContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.UserName == Username);
                var rightsList = (from d in dbContext.UserRights
                                  join e in dbContext.RightsItems on d.RightsId equals e.RightsId
                                  join u in dbContext.Users on d.UserId equals u.UserId
                                  where d.UserId == user.UserId
                                  select new Rights
                                  {
                                      RightsId = d.RightsId,
                                      RightsName = e.RightsName,
                                      IconName = e.IconName,
                                      Read = d.Read,
                                      Write = d.Write,
                                      UserId = d.UserId,
                                      UserName = u.UserName
                                  });

                returnValue = rightsList.ToList();
            }

            return returnValue;
        }

        public bool hasRights(string pageName)
        {
            var returnValue = true;
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var userRights = this.GetRights(userName).Where(e => e.Read || e.Write).ToList();
            if (!userRights.Any(a => a.RightsName == pageName))
            {
                returnValue = false;
            }

            return returnValue;
        }
    }
}