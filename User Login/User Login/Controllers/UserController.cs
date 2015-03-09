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
        //create unique key for user
        private static Guid guid = Guid.NewGuid();
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
                    //login successful
                    FormsAuthentication.SetAuthCookie(user.Email, user.RememberMe);
                    return RedirectToAction("LoggedIn", "User");
                }
                else
                {
                    //incorrect login information
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            /*
             * ****error tracing*****
            */
            //else
            //{
            //    var errors = ModelState.Select(x => x.Value.Errors)
            //               .Where(y => y.Count > 0)
            //               .ToList();
            //}
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
                if (user.Email != null && user.RegisterUser(user.FirstName, user.LastName, user.Password, user.Email, guid.ToString()))
                {
                    //user registration successful
                    //Guid guid = Guid.NewGuid();
                    Manager.EmailManager.SendConfirmationEmail(user.FirstName, user.Email, guid.ToString());
                    return RedirectToAction("Confirmation", "User");
                    //FormsAuthentication.SetAuthCookie(user.Email, user.RememberMe);
                    //return RedirectToAction("Profile", "User");
                }
                //check for duplicate email address
                else if (entities.Tbl_Users.Any(r => r.Email_Id == user.Email))
                {
                    ModelState.AddModelError("", "Account with email address already exists. Login instead.");
                }
                //incorrect information in one or more fields
                else
                {
                    ModelState.AddModelError("", "Registration data is incorrect!");
                }
            }
            return View(user);
        }

        [AllowAnonymous]
        public ActionResult Confirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Verify()
        {
            try
            {
                string guidToTest = Request["id"];
                string email = Request["email"];
                var entities = new Job_Candidate_Application_Entities();

                var model = entities.Tbl_Users.Find(email);
                var is_active = model.Is_Active;

                //user has not confirmed yet
                if (is_active == 0)
                {
                    if (email != null && guidToTest != null)
                    {
                        var user = new Models.User();

                        var guid = model.User_Guid;


                        if (guidToTest == guid.ToString() && is_active == 0)
                        {
                            //email confirmed
                            if (user.Confirmed(email) == true)
                            {
                                Session["verify"] = "Thank you for verifying your email. You can now log in to the account, set up your profile and apply to jobs. The distance between you and your career has never been closer.";
                                return View();
                            }
                            //FormsAuthentication.SetAuthCookie(user.Email, true);
                            //return RedirectToAction("LogIn", "User");
                        }
                        else
                        {
                            Session["verify"] = "Error in verifying your email. Please try again!";
                            return View();
                        }
                    }
                }
                else
                {
                    Session["verify"] = "Email has already been confirmed. Please try to login.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Session["verify"] = ex.Message;
            }

            //something happened. redirect user to home screen
            return RedirectToAction("Index", "Home");
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
                UserProfile user = new UserProfile();

                var entities = new Job_Candidate_Application_Entities();

                //logged in user email
                var email = User.Identity.Name;

                var model = entities.Tbl_Users.Find(email);

                //store user information in UserProfile class
                user.FirstName = model.User_First_Name;
                user.LastName = model.User_Last_Name;
                user.Street = model.User_Street;
                user.City = model.User_City;
                user.State = model.User_State;
                user.Country = model.User_Country;
                user.phone_number = model.User_Phone_Number;
                user.Skills = model.Skills;
                user.Experience_Years = model.Exp_Years;

                return View(user);
            }
            else
                //user not authenticated
                return RedirectToAction("Login", "User");
        } 

        [HttpPost]
        public ActionResult Profile(Models.UserProfile user)
        {
            //logged in user email
            string email = User.Identity.Name;

            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    if (user.UpdateProfile(email, user.FirstName, user.LastName, user.Street, user.City, user.State, user.Country, user.phone_number, user.Experience_Years, user.Skills))
                    {
                        //update successful
                        FormsAuthentication.SetAuthCookie(User.Identity.Name,true);
                        TempData["success"] = "Profile successfully updated!";
                        return RedirectToAction("Profile", "User");
                    }
                    else
                    {
                        //update failed
                        //invalid data in one or more fields
                        //TempData["success"] = null;
                        ModelState.AddModelError("", "Registration data is incorrect!");
                    }
                }

                return View(user);
            }
            else
                //user not authenticated
                return RedirectToAction("Login", "User");
        }
    }
}