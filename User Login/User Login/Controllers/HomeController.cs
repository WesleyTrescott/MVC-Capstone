using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Login.Models;

namespace User_Login.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            //var entities = new EnhancedJobCandidateAppEntities();
            var entities = new Job_Candidate_Application_Entities();
            IEnumerable<Tbl_Jobs> mylist = entities.Tbl_Jobs.ToList();
            var sixRandomFoos = mylist.OrderBy(x => Guid.NewGuid()).Take(6);
            return View(sixRandomFoos);
        }
	}
}