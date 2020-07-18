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
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            var userModel = new UserModel();
            using (var dbContext = new DatabaseContext())
            {
                var users = dbContext.Users.Where(u => !u.IsAdmin).Select(s => new UserMaster
                {
                    UserId = s.UserId,
                    UserName = s.UserName,
                    Password = s.Password,
                    IsActive = s.IsActive
                }).ToList();
                userModel.users = users;
            }
            return View(userModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserMaster userMaster)
        {
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var dupUser = dbContext.Users.FirstOrDefault(u => u.UserName == userMaster.UserName);
                    if (dupUser == null)
                    {
                        var user = new Users();
                        user.UserName = userMaster.UserName;
                        user.Password = userMaster.Password;
                        user.IsActive = userMaster.IsActive;
                        user.IsAdmin = false;
                        dbContext.Users.Add(user);
                        dbContext.SaveChanges();
                        ViewBag.Status = "Created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Status = "Duplicate";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Status = "Failed";
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var userMaster = new UserMaster();
            using (var dbContext = new DatabaseContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.UserId == id);
                userMaster.UserId = user.UserId;
                userMaster.UserName = user.UserName;
                userMaster.Password = user.Password;
                userMaster.IsActive = user.IsActive;
            }

            return View(userMaster);
        }

        [HttpPost]
        public ActionResult Edit(UserMaster userMaster)
        {
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var dupUser = dbContext.Users.FirstOrDefault(u => u.UserName == userMaster.UserName && u.UserId != userMaster.UserId);
                    if (dupUser == null)
                    {
                        var user = dbContext.Users.FirstOrDefault(u => u.UserId == userMaster.UserId);
                        user.UserName = userMaster.UserName;
                        user.Password = userMaster.Password;
                        user.IsActive = userMaster.IsActive;
                        user.IsAdmin = false;
                        dbContext.SaveChanges();
                        ViewBag.Status = "Updated";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Status = "Duplicate";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Status = "Failed";
                return View();
            }
        }

        [HttpPost]
        public JsonResult Delete(int UserId)
        {
            var returnValue = new JSONReturnValue();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var user = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
                    if (user != null)
                    {
                        dbContext.Users.Remove(user);
                        dbContext.SaveChanges();
                        returnValue.Status = true;
                        returnValue.Message = "Record has been deleted successfully";
                    }
                    else
                    {
                        returnValue.Status = false;
                        returnValue.Message = "Record not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Status = false;
                returnValue.Message = "Some error in delete. Please check access or contact administrator.";
            }

            return Json(returnValue);
        }
    }
}