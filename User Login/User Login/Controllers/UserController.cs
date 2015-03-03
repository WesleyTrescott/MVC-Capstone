using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading;
using User_Login.Models;

namespace User_Login.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.LoginUser user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.Email, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Email, user.RememberMe);
                    return RedirectToAction("LoggedIn", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Models.User user)
        {
            var entities = new Job_Candidate_Application_Entities();

            if (ModelState.IsValid)
            {
                if (user.Email != null && user.RegisterUser(user.FirstName, user.LastName, user.Password, user.Email))
                {
                    FormsAuthentication.SetAuthCookie(user.Email, user.RememberMe);
                    //return RedirectToAction("Login", "User");
                    return RedirectToAction("Profile", "User");
                }
                else if (entities.Tbl_Users.Any(r => r.Email_Id == user.Email))
                {
                    ModelState.AddModelError("", "Account with email address already exists. Login instead.");
                }
                else
                {
                    ModelState.AddModelError("", "Registration data is incorrect!");
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult LoggedIn()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public ActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                //var entities = new Job_Candidate_Application_Entities();

                //var name = Membership.GetUser().UserName;

                //var email = Membership.GetUser().Email;

                //var model = (from r in entities.Tbl_Users
                //             where r.Email_Id == Membership.GetUser().Email
                //             select r);

                //return View(entities.Tbl_Users);
                return View();
            }
            else
                return RedirectToAction("Login", "User");
           
            //return View();
        } 

        //[HttpPost]
        //public ActionResult EditProfile(Models.User user)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (user.UserProfile(user.FirstName, user.LastName, user.Street, user.City, user.Street, user.Country, user.Experience_Years, user.Skills))
        //            {
        //                FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);
        //                //return RedirectToAction("Login", "User");
        //                return RedirectToAction("Dashboard", "User");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Registration data is incorrect!");
        //            }
        //        }
        //        return View(user);
        //    }
        //    else
        //        return RedirectToAction("Login", "User");
        //}
    }
}