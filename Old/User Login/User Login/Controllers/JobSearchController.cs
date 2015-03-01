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
        public ActionResult Index()
        {
            var entities = new JobDbEntities();
            return View(entities.Jobs.ToList());
        }
	}
}