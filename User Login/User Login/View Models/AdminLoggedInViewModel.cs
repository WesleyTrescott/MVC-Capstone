using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using User_Login.Models;

namespace User_Login.View_Models
{
    public class AdminLoggedInViewModel
    {
        public IPagedList<Tbl_Users> pagedList { get; set; }

        public IEnumerable<Tbl_Users> recUsers { get; set; }

        public int numPagesRecUsers { get; set; }

        public int numRecUsers { get; set; }

        public string name { get; set; }
    }
}