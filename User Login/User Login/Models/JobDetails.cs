using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class JobDetails
    {
        public List<JobDetail> JobModel { get; set; }
    }

    public class JobDetail
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Skills { get; set; }
        public int? Minimum_Experience_Required { get; set; }
        public int? Maximum_Experience_Required { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public decimal? Pay_Rate { get; set; }
    }
}