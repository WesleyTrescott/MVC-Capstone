using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading;
using PagedList;
using User_Login.Models;
using System.IO;
using User_Login.View_Models;

namespace User_Login.Controllers
{
    public class UserController : Controller
    {
        //create unique key for user
        private static Guid guid = Guid.NewGuid();
        //
        // GET: /User/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.LoginUser user)
        {
            if (ModelState.IsValid)
            {
                if(user.IsAdmin(user.Email, user.Password))
                {
                    //Admin Login
                    FormsAuthentication.SetAuthCookie(user.Email, user.RememberMe);
                    return RedirectToAction("AdminLoggedIn", "User");
                }
                else if (user.IsValid(user.Email, user.Password))
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
        [AllowAnonymous]
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
        public ActionResult LoggedIn(string city, string customer, int? page)
        {
            var entities = new Job_Candidate_Application_Entities();


            if (city != null && (city != "" || city != "Select One"))
            {
                ViewBag.LocationLabel = city;
                ViewBag.Location = (from r in entities.Tbl_Jobs select r.City_Name).Distinct();
            }
            if (customer != null && (customer != "" || customer != "Select One"))
            {
                ViewBag.CustomerLabel = customer;
                ViewBag.Customer = (from r in entities.Tbl_Jobs select r.Customer).Distinct();
            }

            if (page == null && city == null && customer == null)
            {
                ViewBag.Location = (from r in entities.Tbl_Jobs select r.City_Name).Distinct();
                ViewBag.Customer = (from r in entities.Tbl_Jobs select r.Customer).Distinct();
                ViewBag.LocationLabel = "Select One";
                ViewBag.CustomerLabel = "Select One";
            }
            else if ((city == "" || city == "Select One"))
            {
                ViewBag.Location = (from r in entities.Tbl_Jobs select r.City_Name).Distinct();
                ViewBag.LocationLabel = "Select One";
                city = "";
            }
            else if ((customer == "" || customer == "Select One"))
            {
                ViewBag.Customer = (from r in entities.Tbl_Jobs select r.Customer).Distinct();
                ViewBag.CustomerLabel = "Select One";
                customer = "";
            }

            var model = from r in entities.Tbl_Jobs select r;

            if (city != "" || customer != "")
            {
                if (city != "" && customer != "")
                {
                    model = (from r in entities.Tbl_Jobs where r.Customer == customer && r.City_Name == city select r);
                }
                else if (city != "")
                {
                    model = (from r in entities.Tbl_Jobs where r.City_Name == city select r);
                }
                else
                {
                    model = (from r in entities.Tbl_Jobs where r.Customer == customer select r);
                }
            }
            else
            {
                model = from r in entities.Tbl_Jobs where r.Position == "dkjfldjls" select r;
            }

            int pageSize = 3;
            int pageNum = (page ?? 1);

            LoggedInViewModel viewModel = new LoggedInViewModel();

            string userName = User.Identity.Name;
            viewModel.recJobs = getRecommendedJobs(userName, viewModel);

            if (User.Identity.IsAuthenticated)
            {
                viewModel.pagedList = model.OrderBy(p => p.Position).ToPagedList(pageNum, pageSize);
                return View(viewModel);
            }
            else
                return RedirectToAction("Login", "User");
        }

        private static IEnumerable<Tbl_Jobs> getRecommendedJobs(string userName, LoggedInViewModel viewModel)
        {
            var entities = new Job_Candidate_Application_Entities();
            Tbl_Users user = entities.Tbl_Users.Find(userName);
            if (user != null)
            {
                string skills = user.Skills;
                var results = from r in entities.Tbl_Jobs where r.Required_Skills.Contains("skills") select r;
                IList<Tbl_Jobs> recJobsList = results.ToList();
                int numresults = recJobsList.Count;
                int numpages = numresults / 6;
                viewModel.numPagesRecJobs = numpages;
                var randomFoos = recJobsList.OrderBy(x => Guid.NewGuid()).Take((numpages * 6));
                return randomFoos;
            }
            return null;
        }

        [HttpGet]
        public ActionResult AdminLoggedIn()
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            Session["forgotPassword"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(Models.ForgotPassword forgotPassword)
        {
            string email = forgotPassword.EmailId;
            Session["forgotPassword"] = null;

            if (email != null)
            {
                var model = new Job_Candidate_Application_Entities();
                Session["forgotPassword"] = "We sent an email with link to change the password. Please check your email.";

                var user = model.Tbl_Users.Find(email);

                if (user != null)
                {
                    var guid = Guid.NewGuid();              //create unique global id
                    string firstName = user.User_First_Name;
                    if (forgotPassword.updateGuid(email, guid.ToString()))
                    {
                        //guid was updated in database, send email to user
                        Manager.EmailManager.SendForgotPasswordEmail(firstName, email, guid.ToString());
                    }
                    return View();
                }
                else
                    return View();
            }
            else
            {
                ModelState.AddModelError("", "Error occurred. Try again!!");
                return View();
            }

            //something happened, send user back to home page
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangeLogInPassword()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {

            Session["changePassword"] = null;
            try
            {
                string guid = Request["id"];
                string email = Request["email"];
                var entities = new Job_Candidate_Application_Entities();

                if (!User.Identity.IsAuthenticated && guid != null && email != null)
                {
                    var model = new Models.ChangePassword();
                    var verifyEmail = entities.Tbl_Users.Find(email);
                    string isGuidValid = verifyEmail.User_Guid;

                    if (guid != isGuidValid)
                    {
                        ModelState.AddModelError("", "Link is not valid.");
                        return View(model);
                    }
                    else if (verifyEmail != null)
                    {
                        //user has not confirmed yet
                        if (email != null && guid != null)
                        {
                            model.EmailId = email;
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email ID could not be found!");
                        return View(model);
                    }
                }
                else if (User.Identity.IsAuthenticated)
                {
                    return View();
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session["verify"] = ex.Message;
            }

            //something happened. redirect user to home screen
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ChangePassword(Models.ChangePassword model)
        {
            try
            {
                var entities = new Job_Candidate_Application_Entities();
                string email = Request["email"];
                if (ModelState.IsValid)
                {
                    string currentPassword = null;
                    if (User.Identity.IsAuthenticated)
                    {
                        email = User.Identity.Name;
                        currentPassword = model.CurrentPassword;

                        var verifyCurrentPassword = entities.Tbl_Users.Find(email).Password;

                        if (verifyCurrentPassword != Helpers.SHA1.Encode(currentPassword))
                        {
                            ModelState.AddModelError("", "Current password is incorrect! Try again");
                            return View(model);
                        }
                    }
                    string password = model.Password;
                    guid = Guid.NewGuid();      //update change to invalidate change password link in the email

                    //update password in the database
                    if (model.UpdatePassword(email, currentPassword, password, guid.ToString()))
                    {
                        model.EmailId = email;
                        model.CurrentPassword = null;
                        model.Password = null;
                        model.ConfirmPassword = null;
                        Session["changePassword"] = "Password was changed successfully";
                        return View(model);
                    }

                }
                else
                {
                    // ModelState.AddModelError("", "Error occured! Try again!");
                    model.EmailId = email;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                //Session["changePassword"] = ex.Message;
                ModelState.AddModelError("", "Error occured! Try again!");
                return View(model);
            }

            //something happened. display page again
            return View(model);
        }

        [HttpGet]
        public ActionResult UploadResume()
        {
            if (User.Identity.IsAuthenticated)
            {
                UploadResume model = new Models.UploadResume();
                string email = User.Identity.Name;
                model.EmailId = email;
                return View();
            }
            else
                return View("Login");
        }

        [HttpPost]
        public ActionResult UploadResume(HttpPostedFileBase resume)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (resume != null && resume.ContentLength > 0)
                    {
                        var allowedExtension = new[] { ".pdf", ".txt", ".doc", ".docx" };
                        var model = new Models.UploadResume();

                        string email = User.Identity.Name;

                        //name of uploaded document
                        string fileName = Path.GetFileName(resume.FileName);
                        string extension = Path.GetExtension(fileName);
                        
                        if (!allowedExtension.Contains(extension))
                        {
                            ModelState.AddModelError("", "Document not supported. Only upload pdf, txt, doc or docx documents only!");
                            return View();
                        }

                        string path = Path.Combine(Server.MapPath("~/App_Data/Applicant's Resumes"), Path.GetFileName(resume.FileName));
                        resume.SaveAs(path);

                        if (model.StoreResumePath(email, path))
                        {
                            ViewBag.Message = "File uploaded successfully";
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("","No file uploaded!");
                        return View();
                    }
                }
                else
                    return View("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred! Try again!");
                return View();
            }

            //something happened, redirect user back to home page.
            return RedirectToAction("Index", "Home");
        }
    }
}