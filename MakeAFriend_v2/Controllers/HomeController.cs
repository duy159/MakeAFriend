using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MakeAFriend_v2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(bool failedlogin = false, bool failedregister = false)
        {
            ViewBag.LoginFailed = failedlogin;
            ViewBag.RegisterFailed = failedregister;

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Category", "Home");
            }
            return View();
        }

        public ActionResult IndexLoginValidate()
        {
            return RedirectToAction("Index", new { failedlogin = true, failedregister = false });
        }

        public ActionResult IndexRegisterValidate()
        {
            return RedirectToAction("Index", new { failedlogin = false, failedregister = true });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Chat()
        {
            ViewBag.Message = "Your chat page";
            return View();
        }

        public ActionResult Category()
        {
            ViewBag.Message = "Your category page";
            return View();
        }
    }
}