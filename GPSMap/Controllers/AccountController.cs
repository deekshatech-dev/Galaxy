using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using GPSDB;
using GPSMap.Models;

namespace GPSMap.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("Login");
        }
        // GET: Account
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new DatabaseContext())
                {
                    var user = dbContext.Users.FirstOrDefault(x => x.Password == model.Password && x.UserName == model.UserName);
                    if (user != null)
                    {
                        //set cookie for logged in user
                        FormsAuthentication.SetAuthCookie(model.UserName.ToLower(), true);
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("error","Please enter valid username and password");
                    }
                }
            }
            return View(model);
        }
    }
}