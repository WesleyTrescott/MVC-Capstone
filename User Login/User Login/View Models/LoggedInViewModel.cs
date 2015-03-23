using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using User_Login.Models;

namespace User_Login.View_Models
{
    public class LoggedInViewModel
    {
        public IPagedList<Tbl_Jobs> pagedList { get; set; }
        public IEnumerable<Tbl_Jobs> recJobs { get; set; }

        public int numPagesRecJobs { get; set; }
    }
}