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
        public ActionResult LoggedIn(/*string city, string customer, int? page*/)
        {
            //var entities = new Job_Candidate_Application_Entities();


            //if (city != null && (city != "" || city != "Select One"))
            //{
            //    ViewBag.LocationLabel = city;
            //    ViewBag.Location = (from r in entities.Tbl_Jobs select r.City_Name).Distinct();
            //}
            //if (customer != null && (customer != "" || customer != "Select One"))
            //{
            //    ViewBag.CustomerLabel = customer;
            //    ViewBag.Customer = (from r in entities.Tbl_Jobs select r.Customer).Distinct();
            //}

            //if (page == null && city == null && customer == null)
            //{
            //    ViewBag.Location = (from r in entities.Tbl_Jobs select r.City_Name).Distinct();
            //    ViewBag.Customer = (from r in entities.Tbl_Jobs select r.Customer).Distinct();
            //    ViewBag.LocationLabel = "Select One";
            //    ViewBag.CustomerLabel = "Select One";
            //}
            //else if ((city == "" || city == "Select One"))
            //{
            //    ViewBag.Location = (from r in entities.Tbl_Jobs select r.City_Name).Distinct();
            //    ViewBag.LocationLabel = "Select One";
            //    city = "";
            //}
            //else if ((customer == "" || customer == "Select One"))
            //{
            //    ViewBag.Customer = (from r in entities.Tbl_Jobs select r.Customer).Distinct();
            //    ViewBag.CustomerLabel = "Select One";
            //    customer = "";
            //}

            //var model = from r in entities.Tbl_Jobs select r;

            //if (city != "" || customer != "")
            //{
            //    if (city != "" && customer != "")
            //    {
            //        model = (from r in entities.Tbl_Jobs where r.Customer == customer && r.City_Name == city select r);
            //    }
            //    else if (city != "")
            //    {
            //        model = (from r in entities.Tbl_Jobs where r.City_Name == city select r);
            //    }
            //    else
            //    {
            //        model = (from r in entities.Tbl_Jobs where r.Customer == customer select r);
            //    }
            //}
            //else
            //{
            //    model = from r in entities.Tbl_Jobs where r.Position == "dkjfldjls" select r;
            //}

            //int pageSize = 3;
            //int pageNum = (page ?? 1);

            LoggedInViewModel viewModel = new LoggedInViewModel();

            string userName = User.Identity.Name;
            viewModel.recJobs = getRecommendedJobs(userName, viewModel);

            if (viewModel.numPagesRecJobs == 0)
            {
                var jobentities = new Job_Candidate_Application_Entities();
                IList<Tbl_Jobs> mylist = jobentities.Tbl_Jobs.ToList();
                var sixRandomFoos = mylist.OrderBy(x => Guid.NewGuid()).Take(6);
                viewModel.recJobs = sixRandomFoos;
            }

            if (User.Identity.IsAuthenticated)
            {
                //viewModel.pagedList = model.OrderBy(p => p.Position).ToPagedList(pageNum, pageSize);
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
                viewModel.name = user.User_First_Name;
                string skills = user.Skills;
                var results = from r in entities.Tbl_Jobs where r.Required_Skills.Contains("skills") select r;
                IList<Tbl_Jobs> recJobsList = results.ToList();
                int numresults = recJobsList.Count;
                int numpages = numresults / 6;
                viewModel.numRecJobs = 6 * numpages;
                viewModel.numPagesRecJobs = numpages;
                var randomFoos = recJobsList.OrderBy(x => Guid.NewGuid()).Take((numpages * 6));
                return randomFoos;
            }
            return null;
        }

        public ActionResult UserJobSearch(string city, string customer, int? page, FormCollection form)
        {
            var entities = new Job_Candidate_Application_Entities();

            string selectedSkills = form["skills"];
            string selectedCity = form["city"];
            string selectedCustomer = form["customer"];
            List<string> skillsList = null;

            if (selectedCity == null && (city != null && city != "" && city != "Select One"))
                selectedCity = city;
            if (selectedCustomer == null && (customer != null && customer != "" && customer != "Select One"))
                selectedCustomer = customer;

            if(selectedSkills != null)
            {
                skillsList = selectedSkills.Split(',').ToList();
            }

            var model = from r in entities.Tbl_Jobs select r;


            if ((selectedCity == null || selectedCity == "") && (selectedCustomer == null || selectedCustomer == "") && (skillsList == null))
            {
                model = from r in entities.Tbl_Jobs where r.Position == "dkjfldjlsdlfjljwljlwjrow" select r;
            }
            else if ((!(selectedCity == null || selectedCity == "")) && (selectedCustomer == null || selectedCustomer == "") && (skillsList == null))
            {
                model = (from r in entities.Tbl_Jobs where r.City_Name == selectedCity select r);
            }
            else if ((selectedCity == null || selectedCity == "") && (!(selectedCustomer == null || selectedCustomer == "")) && (skillsList == null))
            {
                model = (from r in entities.Tbl_Jobs where r.Customer == selectedCustomer select r);
            }
            else if ((selectedCity == null || selectedCity == "") && (selectedCustomer == null || selectedCustomer == "") && (!(skillsList == null)))
            {
               List<Tbl_Jobs> resultslist = new List<Tbl_Jobs>();
                foreach (string item in skillsList)
                {
                    model = (from r in entities.Tbl_Jobs where r.Required_Skills.Contains(item) select r).Distinct();
                    foreach(var entry in model)
                    {
                        resultslist.Add(entry);
                    }
                }
                model = resultslist.Distinct().AsQueryable();
            }
            else if ((!(selectedCity == null || selectedCity == "")) && (!(selectedCustomer == null || selectedCustomer == "")) && (skillsList == null))
            {
                model = (from r in entities.Tbl_Jobs where r.Customer == selectedCustomer && r.City_Name == selectedCity select r);
            }
            else if ((!(selectedCity == null || selectedCity == "")) && (selectedCustomer == null || selectedCustomer == "") && (!(skillsList == null)))
            {
                List<Tbl_Jobs> resultslist = new List<Tbl_Jobs>();
                foreach (string item in skillsList)
                {
                    model = (from r in entities.Tbl_Jobs where r.City_Name == selectedCity && r.Required_Skills.Contains(item) select r).Distinct();
                    foreach (var entry in model)
                    {
                        resultslist.Add(entry);
                    }
                }
                model = resultslist.Distinct().AsQueryable();
            }
            else if ((!(selectedCity == null || selectedCity == "")) && (!(selectedCustomer == null || selectedCustomer == "")) && (!(skillsList == null)))
            {
                List<Tbl_Jobs> resultslist = new List<Tbl_Jobs>();
                foreach (string item in skillsList)
                {
                    model = (from r in entities.Tbl_Jobs where r.City_Name == selectedCity && r.Customer == selectedCustomer && r.Required_Skills.Contains(item) select r).Distinct();
                    foreach (var entry in model)
                    {
                        resultslist.Add(entry);
                    }
                }
                model = resultslist.Distinct().AsQueryable();
            }
            else if ((selectedCity == null || selectedCity == "") && (!(selectedCustomer == null || selectedCustomer == "")) && (!(skillsList == null)))
            {
                List<Tbl_Jobs> resultslist = new List<Tbl_Jobs>();
                foreach (string item in skillsList)
                {
                    model = (from r in entities.Tbl_Jobs where r.Customer == selectedCustomer && r.Required_Skills.Contains(item) select r).Distinct();
                    foreach (var entry in model)
                    {
                        resultslist.Add(entry);
                    }
                }
                model = resultslist.Distinct().AsQueryable();
            }
            else
            {
                model = from r in entities.Tbl_Jobs where r.Position == "dkjfldjlsdlfjljwljlwjrow" select r;
            }


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

            int pageSize = 3;
            int pageNum = (page ?? 1);

            LoggedInViewModel viewModel = new LoggedInViewModel();
            ViewBag.skills = getSkillsList(null);
            viewModel.pagedList = model.OrderBy(p => p.Position).ToPagedList(pageNum, pageSize);
            
            return PartialView("UserJobSearch", viewModel);
        }

        private MultiSelectList getSkillsList(string[] selectedValues)
        {
            var skillsList = new List<string>();
            skillsList.Add(".NET Framework");
            skillsList.Add("Agile/Scrum Development");
            skillsList.Add("Ajax");
            skillsList.Add("Algorithms/Data Structures");
            skillsList.Add("Analytics");
            skillsList.Add("Android Programming ");
            skillsList.Add("Business Best Practices");
            skillsList.Add("Business Process Improvement");
            skillsList.Add("Business Professionalism");
            skillsList.Add("C Programming");       
            skillsList.Add("C# Programming");
            skillsList.Add("C++ Programming");
            skillsList.Add("Cascading Style Sheets (CSS)");
            skillsList.Add("Corporate Best Practices");
            skillsList.Add("Corporate Management");
            skillsList.Add("Critical Thinking");
            skillsList.Add("Database Design");
            skillsList.Add("From-Scratch Development");
            skillsList.Add("Frontend/UI Development");
            skillsList.Add("Git/Github");
            skillsList.Add("HTML");
            skillsList.Add("Human Resources");
            skillsList.Add("Industrial Engineering");
            skillsList.Add("Information Technology Management");
            skillsList.Add("iOS Programming");
            skillsList.Add("Java/Java Framework");
            skillsList.Add("JavaScript Programming");
            skillsList.Add("Mobile Application Development");
            skillsList.Add("Problem Solving");
            skillsList.Add("Professional Oral/Written Communication");
            skillsList.Add("Project Build Lifecycle Management");
            skillsList.Add("Project Management");
            skillsList.Add("Ruby on Rails Framework");
            skillsList.Add("Software Design Patterns");
            skillsList.Add("Software Documentation");
            skillsList.Add("Software Lifecycle Management");
            skillsList.Add("Software Maintenance");
            skillsList.Add("Software Testing");
            skillsList.Add("Source Control Management");
            skillsList.Add("SQL Programming");
            skillsList.Add("Subversion (SVN)");
            skillsList.Add("Technical Support");
            skillsList.Add("Web Development");
            skillsList.Add("Web Services");
            skillsList.Add("XML Programming");

            return new MultiSelectList(skillsList, selectedValues);
        }

        [HttpGet]
        public ActionResult AdminLoggedIn(int? page)
        {
            var entities = new Job_Candidate_Application_Entities();
            var model = from r in entities.Tbl_Users select r;
            int pageSize = 3;
            int pageNum = (page ?? 1);

            AdminLoggedInViewModel viewModel = new AdminLoggedInViewModel();

           // string userName = User.Identity.Name;
            //viewModel.recJobs = getRecommendedJobs(userName, viewModel);

            if (viewModel.numPagesRecUsers == 0)
            {
                var userentities = new Job_Candidate_Application_Entities();
                IList<Tbl_Users> mylist = userentities.Tbl_Users.ToList();
                //var sixRandomFoos = mylist.OrderBy(x => Guid.NewGuid()).Take(6);
                viewModel.recUsers = mylist;
            }
            if (User.Identity.IsAuthenticated)
            {
                viewModel.pagedList = model.OrderBy(p => p.User_Last_Name).ToPagedList(pageNum, pageSize);
                return View(viewModel);
            }
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
        public ActionResult DeleteUser(string email)
        {
            Models.User user = new Models.User();
            user.DeleteUser(email);
            return View();
        }

        public ActionResult DeActivateUser(string email)
        {
            Models.User user = new Models.User();
            user.DeActivateUser(email);
            return View();
        }

        public ActionResult ActivateUser(string email)
        {
            Models.User user = new Models.User();
            user.ActivateUser(email);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            Session["forgotPassword"] = null;
            Session["previousPage"] = Request.UrlReferrer.ToString();
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
            Session["changePassword"] = null;
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
                Session["uploadResume"] = null;
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
                        var entities = new Job_Candidate_Application_Entities();
                        var allowedExtension = new[] { ".pdf", ".txt", ".doc", ".docx" };
                        var model = new Models.UploadResume();

                        string email = User.Identity.Name;
                        var user = entities.Tbl_Users.Find(email);

                        string lastName = user.User_Last_Name;                  //user last name
                        string firstName = user.User_First_Name[0].ToString();  //user first name initial
                        string date = DateTime.Now.ToString();                  //current date and time to make file unique

                        //remove unsupported characters in file name
                        date = date.Replace('/', '-');
                        date = date.Replace(':', '.');

                        //name of uploaded document
                        string fileName = Path.GetFileName(resume.FileName);
                        string extension = Path.GetExtension(fileName);
                        
                        //validate extension of uploaded file
                        if (!allowedExtension.Contains(extension))
                        {
                            ModelState.AddModelError("", "Document not supported. Only upload pdf, txt, doc or docx documents only!");
                            Session["uploadResume"] = null;
                            return View();
                        }

                        string tempFileName = fileName;
                        
                        //unique file name
                        fileName = lastName + "_" + firstName + "_" + date + "_" + tempFileName;
                        
                        string path = Path.Combine(Server.MapPath("~/App_Data/Applicant's Resumes"), fileName);
                        resume.SaveAs(path);

                        if (model.StoreResumePathInUserProfile(email, path))
                        {
                            Session["uploadResume"] = "File uploaded successfully";
                            return View();
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error occured in uploading resume. Try again.");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("","No file uploaded!");
                        Session["uploadResume"] = null;
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