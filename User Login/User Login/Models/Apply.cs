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
        public int? ExperienceYears { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public bool UseExistingResume { get; set; }
        public string ResumePath { get; set; }

        public bool hasApplied(string email, int jobID)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
            {
                //check if user has applied for job
                string _sql = @"SELECT * FROM [Tbl_Job_Application] WHERE [Job_Id] = @jobID AND [Email_Id] = @emailID";
                var cmd = new SqlCommand(_sql, cn);
                //cmd.Parameters.Add(new SqlParameter("@jobID", SqlDbType.Int)).Value = jobID;
                //cmd.Parameters.Add(new SqlParameter("@emailID", SqlDbType.NVarChar)).Value = email;
                
                cmd.Parameters.AddWithValue("@jobID", jobID);
                cmd.Parameters.AddWithValue("@emailID", email);

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

        public bool submitApplication(string email, int jobID, string firstName, string lastName, string street, string city, string state, string country, string phoneNumber, string skills, int? experienceYears, string resumePath)
        {
            try
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    //check if user has applied for job
                    string _sql = @"INSERT INTO [Tbl_Job_Application] (Job_Id, Email_Id, User_First_Name, User_Last_Name, User_Street, User_City, User_State, User_Country, User_Phone_Number, Skills, Exp_Years, Resume_Upload) values (@jobId, @emailId, @firstName, @lastName, @street, @city, @state, @country, @phoneNumber, @skills, @expYears, @resumePath)";

                    var cmd = new SqlCommand(_sql, cn);
                    //cmd.Parameters.Add(new SqlParameter("@jobID", SqlDbType.Int)).Value = jobID;
                    //cmd.Parameters.Add(new SqlParameter("@emailID", SqlDbType.VarChar)).Value = email;
                    cmd.Parameters.AddWithValue("@jobId", jobID);
                    cmd.Parameters.AddWithValue("@emailId", email);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@street", street);
                    cmd.Parameters.AddWithValue("@city", city);
                    cmd.Parameters.AddWithValue("@state", state);
                    cmd.Parameters.AddWithValue("@country", country);
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                   
                    if (skills != null)
                    {
                        cmd.Parameters.AddWithValue("@skills", skills);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@skills", DBNull.Value);
                    }

                    if (experienceYears != null)
                    {
                        cmd.Parameters.AddWithValue("@expYears", experienceYears);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@expYears", DBNull.Value);
                    }

                    if (resumePath != null)
                    {
                        cmd.Parameters.AddWithValue("@resumePath", resumePath);
                    }
                    else
                    {
                        cmd.Parameters.Add("@resumePath", DBNull.Value);
                        //return false;
                    }

                    
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