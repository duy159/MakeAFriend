using MakeAFriend_v2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MakeAFriend_v2.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext _db = ApplicationDbContext.Create();
        public ActionResult Index(bool failedlogin = false, bool failedregister = false)

        {
            ViewBag.LoginFailed = failedlogin;
            ViewBag.RegisterFailed = failedregister;

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Lobby", "Home");
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
            changeStatus(User.Identity.GetUserId(), User.Identity.GetUserName(), "Online");
            return View();
        }

        public ActionResult Lobby()
        {
            ViewBag.Message = "Your lobby page";
            return View();
        }

        // Friend functions
        public ActionResult addFriends(string name, string otherusername)
        {
            var user = from m in _db.Users
                       where m.UserName == name
                       select m;
            var friend = from m in _db.Users
                         where m.UserName == otherusername
                         select m;

            Friends f = new Friends();
            f.Id = user.First<ApplicationUser>().Id;
            f.FriendId = friend.First<ApplicationUser>().Id;
            f.UserName = friend.First<ApplicationUser>().UserName;
            f.UserStatus = friend.First<ApplicationUser>().UserStatus;
            _db.MyFriends.Add(f);
            _db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        // Friend functions
        public ActionResult getFriends(string name)
        {
            var friends = from m in _db.Users
                          where m.UserName == name
                          select m;
            int length = friends.ToArray().Length;
            int i = 0;
            string[,] friendsStr = new string[2, length];

            foreach (ApplicationUser f in friends.ToArray<ApplicationUser>())
            {
                friendsStr[0,i] = f.UserName;
                friendsStr[1,i] = f.UserStatus;
                i++;
            }

            return Json(friendsStr, JsonRequestBehavior.AllowGet);
        }

        // Change functions
        public void changeStatus(string id, string name, string status)
        {
            var user = new ApplicationUser() { Id = id, UserName = name, UserStatus = status };

            _db.Users.Attach(user);
            _db.Entry(user).Property(x => x.UserStatus).IsModified = true;
            _db.SaveChanges();
        }

    }
}