﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using User_Login.Models;
using Recaptcha;

namespace User_Login.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            var entities = new Job_Candidate_Application_Entities();
            IList<Tbl_Jobs> mylist = entities.Tbl_Jobs.ToList();
            var sixRandomFoos = mylist.OrderBy(x => Guid.NewGuid()).Take(6);
            return View(sixRandomFoos);
        }

        [HttpPost]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult ContactUs(Models.ContactModel model, bool captchaValid, string captchaErrorMessage)
        {
            if (captchaValid)
            {
                if (model.firstname != null && model.lastname != null && model.email != null && model.message != null)
                {
                    Manager.EmailManager.SentContactEmail(model);
                    return RedirectToAction("Confirmation", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("recaptcha", captchaErrorMessage);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult Confirmation()
        {
            return View();
        }
	}
}