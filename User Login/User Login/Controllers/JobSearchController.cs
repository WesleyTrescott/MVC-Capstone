using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Login.Models;

namespace User_Login.Controllers
{
    public class JobSearchController : Controller
    {
        //
        // GET: /JobSearch/
        public ActionResult Index(string city, string customer)
        {
           // var entities = new JobDbEntities();
            var entities = new Job_Candidate_Application_Entities();

            ViewBag.Location = (from r in entities.Tbl_Jobs
                               select r.City_Name).Distinct();

            ViewBag.Customer = (from r in entities.Tbl_Jobs
                             select r.Customer).Distinct();

            var model = (from r in entities.Tbl_Jobs
                        where r.Customer == customer || r.City_Name == city
                        select r);

            if (city != null || customer != null)
                return View(model);
            else
                return View(entities.Tbl_Jobs.ToList());
        }

        public ActionResult Details(int? id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            //var entities = new JobDbEntities();
            var entities = new Job_Candidate_Application_Entities();
            Session["hasApplied"] = "";
            //Job jobs = entities.Jobs.Where(x=>x.Job_Id == id).Select(x =>
            //    new Job
            //    )

            Tbl_Jobs jobs = entities.Tbl_Jobs.Find(id);
            var user = new Models.Apply();
            var email = User.Identity.Name;

            if (jobs == null)
            {
                return HttpNotFound();
            }
            else if (user.hasApplied(email,jobs.Job_Id) == true)
            {
                Session["hasApplied"] = "You have applied for this job";
            }

            return View(jobs);
        }

        [HttpGet]
        public ActionResult Apply(int? id = 0)
        {
            if (id == null)
                return RedirectToAction("Index", "JobSearch");

            if (User.Identity.IsAuthenticated)
            {
                var applyJob = new Models.Apply();
                var email = User.Identity.Name;

                var entities = new Job_Candidate_Application_Entities();

                var job = entities.Tbl_Jobs.Find(id);
                var userInfo = entities.Tbl_Users.Find(email);

                if (applyJob.hasApplied(email, job.Job_Id) == false)
                {
                    applyJob.JobPosition = job.Position;
                    applyJob.JobId = job.Job_Id;
                    applyJob.EmailId = userInfo.Email_Id;
                    applyJob.FirstName = userInfo.User_First_Name;
                    applyJob.LastName = userInfo.User_Last_Name;
                    applyJob.Street = userInfo.User_Street;
                    applyJob.City = userInfo.User_City;
                    applyJob.State = userInfo.User_State;
                    applyJob.Country = userInfo.User_Country;
                    applyJob.PhoneNumber = userInfo.User_Phone_Number;
                    applyJob.Skills = userInfo.Skills;
                    applyJob.ExperienceYears = userInfo.Exp_Years;
                    //if (userInfo.Resume_Upload == null)
                    //{
                    //    applyJob.ResumePath = null;
                    //}
                    //else
                    //{
                        applyJob.ResumePath = userInfo.Resume_Upload;
                    //}

                    //var errors = ModelState.Select(x => x.Value.Errors)
                    //           .where(y => y.count > 0)
                    //           .tolist();

                    return View(applyJob);
                }
                else
                {
                    Session["submitApplication"] = "You have already applied for the job";
                    return View("SubmitApplication");
                }
            }
            else
                return RedirectToAction("Login", "User");
        }

        /*
        [HttpPost]
        public ActionResult Apply(Models.Apply applyJob, HttpPostedFileBase resume)
        {
            //var user = new Models.Apply();
            string email = applyJob.EmailId;
            int jobId = applyJob.JobId;
            string firstName = applyJob.FirstName;
            string lastName = applyJob.LastName;
            string street = applyJob.Street;
            string city = applyJob.City;
            string state = applyJob.State;
            string country = applyJob.Country;
            int? phoneNumber = applyJob.PhoneNumber;
            string skills = applyJob.Skills;
            int? experienceYears = applyJob.ExperienceYears;
            
            bool useExistingResume = applyJob.UseExistingResume;
            string resumePath = null;

            if (useExistingResume)
            {
                var entities = new Job_Candidate_Application_Entities();
                var user = entities.Tbl_Users.Find(email);
                resumePath = user.Resume_Upload;
            }
            else if (resume != null)
            {
                var entities = new Job_Candidate_Application_Entities();
                var user = entities.Tbl_Users.Find(email);
                var allowedExtension = new[] { ".pdf", ".txt", ".doc", ".docx" };
                var model = new Models.UploadResume();

                string firstNameInitial = user.User_First_Name[0].ToString();  //user first name initial
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
                    return View();
                }

                string tempFileName = fileName;
                        
                //unique file name
                fileName = lastName + "_" + firstNameInitial + "_" + date + "_" + tempFileName;
                        
                resumePath = Path.Combine(Server.MapPath("~/App_Data/Applicant's Resumes"), fileName);
                resume.SaveAs(resumePath);

                //if (model.StoreResumePathInJobApplicationProfile(email, path))
                //{
                //    return View();
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Error occured in uploading resume. Try again.");
                //    return View();
                //}

                if (ModelState.IsValid)
                {
                    if (applyJob.submitApplication(email, jobId, firstName, lastName, street, city, state, country, phoneNumber, skills, experienceYears, resumePath))
                    {
                        Session["submitApplication"] = "Application was submitted successfully. Thank you for your interest.";
                    }
                    else
                    {
                        Session["submitApplication"] = "There was an error in submitting your application. Please try again. Sorry for the inconvenience";
                    }

                    return View("SubmitApplication");
                }
            }
            else
                return RedirectToAction("Index", "JobSearch");

            return RedirectToAction("Index", "JobSearch");
        }
         * */

        [HttpPost]
        public ActionResult Apply(Models.Apply applyJob, HttpPostedFileBase resume)
        {
            //var user = new Models.Apply();
            string email = applyJob.EmailId;
            int jobId = applyJob.JobId;
            string firstName = applyJob.FirstName;
            string lastName = applyJob.LastName;
            string street = applyJob.Street;
            string city = applyJob.City;
            string state = applyJob.State;
            string country = applyJob.Country;
            int? phoneNumber = applyJob.PhoneNumber;
            string skills = applyJob.Skills;
            int? experienceYears = applyJob.ExperienceYears;

            bool useExistingResume = applyJob.UseExistingResume;
            string resumePath = null;

            if (User.Identity.IsAuthenticated)
            {
                if (useExistingResume)
                {
                    var entities = new Job_Candidate_Application_Entities();
                    var user = entities.Tbl_Users.Find(email);
                    resumePath = user.Resume_Upload;
                }
                else if (resume != null)
                {
                    var entities = new Job_Candidate_Application_Entities();
                    var user = entities.Tbl_Users.Find(email);
                    var allowedExtension = new[] { ".pdf", ".txt", ".doc", ".docx" };
                    var model = new Models.UploadResume();

                    string firstNameInitial = user.User_First_Name[0].ToString();  //user first name initial
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
                        return View();
                    }

                    string tempFileName = fileName;

                    //unique file name
                    fileName = lastName + "_" + firstNameInitial + "_" + date + "_" + tempFileName;

                    resumePath = Path.Combine(Server.MapPath("~/App_Data/Applicant's Resumes"), fileName);
                    resume.SaveAs(resumePath);
                }

                //if (model.StoreResumePathInJobApplicationProfile(email, path))
                //{
                //    return View();
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Error occured in uploading resume. Try again.");
                //    return View();
                //}

                if (ModelState.IsValid)
                {
                    if (applyJob.submitApplication(email, jobId, firstName, lastName, street, city, state, country, phoneNumber, skills, experienceYears, resumePath))
                    {
                        Session["submitApplication"] = "Application was submitted successfully. Thank you for your interest.";
                    }
                    else
                    {
                        Session["submitApplication"] = "There was an error in submitting your application. Please try again. Sorry for the inconvenience";
                    }

                    return View("SubmitApplication");
                }
                else
                {
                    ModelState.AddModelError("", "Error submitting application. Make sure required fields are not empty");
                }

                //error tracing
                //var errors = ModelState.Select(x => x.Value.Errors)
                //            .Where(y => y.Count > 0)
                //           .ToList();
            }
            else
                return RedirectToAction("Index", "JobSearch");

            return RedirectToAction("Index", "JobSearch");
        }

        //public ActionResult Details(int id = 0)
        //{
        //    if (id == 0)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    JobDetails details = new JobDetails();
        //    List<JobDetail> job = new List<JobDetail>();

        //    job = GetJobDetails(id);

           
        //    details.JobModel = job;
        //    return View(details);
        //}

        public List<JobDetail> GetJobDetails(int id)
        {
            List<JobDetail> job = new List<JobDetail>();
            var entities = new JobDbEntities();

            var job_details = entities.Jobs.Where(p => p.Job_Id == id);

            foreach (var item in job_details)
            {
                job.Add(new JobDetail
                {
                    Id = item.Job_Id,
                    Position = item.Job_Position,
                    Description = item.Job_Description,
                    Country = item.Country,
                    State = item.State,
                    City = item.City,
                    Minimum_Experience_Required = item.Min_Exp_Req,
                    Maximum_Experience_Required = item.Max_Exp_Req,
                    Start_Date = item.Start_Date,
                    End_Date = item.End_Date,
                    Skills = item.Skills,
                    Pay_Rate = item.PayRate
                });
            }
            return job;
        }
	}
}