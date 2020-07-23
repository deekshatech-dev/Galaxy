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
    [Authorize]
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            // Logger.LoggedMe(Convert.ToInt32(Session["UserId"]), "User", "Index", "Get");
            var userModel = this.GetUsers();

            return View(userModel);
        }

        public UserModel GetUsers()
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
            return userModel;
        }


        public ActionResult Create()
        {
            // Logger.LoggedMe(Convert.ToInt32(Session["UserId"]), "User", "Create", "Get");
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserMaster userMaster)
        {
            try
            {
                // Logger.LoggedMe(Convert.ToInt32(Session["UserId"]), "User", "Create", "Post");
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
                        EnsureUserRights(user.UserId);
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
            // Logger.LoggedMe(Convert.ToInt32(Session["UserId"]), "User", "Edit", "Get");
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
                // Logger.LoggedMe(Convert.ToInt32(Session["UserId"]), "User", "Edit", "Post");
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
                        EnsureUserRights(user.UserId);
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

                        // Delete User Rights
                        var userRights = dbContext.UserRights.Where(u => u.UserId == UserId);
                        dbContext.UserRights.RemoveRange(userRights);

                        dbContext.SaveChanges();
                        ViewBag.Users = this.GetUsers();
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

        public bool EnsureUserRights(int UserId)
        {
            var returnValue = false;
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    if (!dbContext.UserRights.Any(u => u.UserId == UserId))
                    {
                        var rights = dbContext.RightsItems.ToList();
                        foreach (var item in rights)
                        {
                            var userRight = new UserRights();
                            userRight.UserId = UserId;
                            userRight.RightsId = item.RightsId;
                            userRight.Read = true;
                            userRight.Write = true;
                            userRight.CreatedDateTime = System.DateTime.Now;
                            userRight.CreatedBy = 1;
                            dbContext.UserRights.Add(userRight);
                        }

                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.Helper.LogFile(ex, Server.MapPath("~/Error"));
                throw ex;
            }

            return returnValue;
        }

        [HttpGet]
        public ViewResult AccessRights(int id)
        {
            var rightModel = new RightsModel();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var query = (from d in dbContext.UserRights
                                 join e in dbContext.RightsItems on d.RightsId equals e.RightsId
                                 join u in dbContext.Users on d.UserId equals u.UserId
                                 where d.UserId == id
                                 select new Rights
                                 {
                                     RightsId = d.RightsId,
                                     RightsName = e.RightsName,
                                     Read = d.Read,
                                     Write = d.Write,
                                     UserId = d.UserId,
                                     UserName = u.UserName
                                 });
                    rightModel.rights = query.ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.Helper.LogFile(ex, Server.MapPath("~/Error"));
                throw ex;
            }

            return View(rightModel);
        }

        [HttpPost]
        public JsonResult AccessRights(RightsModel model)
        {
            var returnValue = new JSONReturnValue();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    int userId = model.rights[0].UserId;
                    var userRights = dbContext.UserRights.Where(u => u.UserId == userId).ToList();
                    dbContext.UserRights.RemoveRange(userRights);

                    foreach (var item in model.rights)
                    {
                        var userRight = new UserRights();
                        userRight.UserId = item.UserId;
                        userRight.RightsId = item.RightsId;
                        userRight.Read = item.Read;
                        userRight.Write = item.Write;
                        userRight.CreatedDateTime = System.DateTime.Now;
                        userRight.CreatedBy = 1;
                        dbContext.UserRights.Add(userRight);
                    }

                    dbContext.SaveChanges();
                }

                returnValue.Status = true;
                return Json(returnValue);
            }
            catch (Exception ex)
            {
                Helper.Helper.LogFile(ex, Server.MapPath("~/Error"));
                returnValue.Status = false;
                returnValue.Message = ex.Message;
                return Json(returnValue);
            }
        }
    }
}