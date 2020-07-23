using GPSDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Helper
{
    public static class Logger
    {
        public static void LoggedMe(int userId, string controller, string action, string type)
        {
            using (var dbContext = new DatabaseContext())
            {
                var log = new Log();
                log.UserId = userId;
                log.Controller = controller;
                log.Action = action;
                log.Type = type;
                log.CreatedOn= System.DateTime.Now;
                dbContext.Log.Add(log);
                // dbContext.SaveChanges();
            }
        }
    }
}