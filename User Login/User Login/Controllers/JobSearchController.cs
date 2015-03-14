using System;
using System.Collections.Generic;
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
            //Job jobs = entities.Jobs.Where(x=>x.Job_Id == id).Select(x =>
            //    new Job
            //    )

            Tbl_Jobs jobs = entities.Tbl_Jobs.Find(id);

            if (jobs == null)
            {
                return HttpNotFound();
            }

            return View(jobs);
        }

        public ActionResult Apply(int? id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                var entities = new Job_Candidate_Application_Entities();

                var job = entities.Tbl_Jobs.Find(id);
                return View();

            }
            else
                return RedirectToAction("Login", "User");
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