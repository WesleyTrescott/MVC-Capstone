using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class Apply
    {
        public int JobId { get; set; }
        public string EmailId { get; set; }

        [Display(Name = "Job Position")]
        public string JobPosition { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Skills")]
        public string Skills { get; set; }

        [Display(Name = "Years of Experience")]
        public int? Experience_Years { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public int? phone_number { get; set; }

        public bool hasApplied(string email, int jobID)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
            {
                //check if user has applied for job
                string _sql = @"SELECT * FROM [Tbl_Job_Application] WHERE [Job_Id] = @jobID AND [Email_Id] = @emailID";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters.Add(new SqlParameter("@jobID", SqlDbType.Int)).Value = jobID;
                cmd.Parameters.Add(new SqlParameter("@emailID", SqlDbType.NVarChar)).Value = email;
                
                cn.Open();
                var reader = cmd.ExecuteReader();
                
                if (reader.HasRows)     
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }

        public bool submitApplication(string email, int jobID)
        {
            try
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    //check if user has applied for job
                    string _sql = @"INSERT INTO [Tbl_Job_Application] (Job_Id, Email_Id) values (@jobId, @emailId)";

                    var cmd = new SqlCommand(_sql, cn);
                    //cmd.Parameters.Add(new SqlParameter("@jobID", SqlDbType.Int)).Value = jobID;
                    //cmd.Parameters.Add(new SqlParameter("@emailID", SqlDbType.VarChar)).Value = email;
                    cmd.Parameters.AddWithValue("@jobId", jobID);
                    cmd.Parameters.AddWithValue("@emailId", email);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}