using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class UploadResume
    {
        public string EmailId { get; set; }

        public string ResumePath { get; set; }

        public bool StoreResumePathInUserProfile(string email, string resumePath)
        {
            try
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    string sqlStmt = null;

                    sqlStmt = @"UPDATE [Tbl_Users] set [Resume_Upload] = @resumePath where [Email_Id] = @email";           
                    
                    var cmd = new SqlCommand(sqlStmt, cn);

                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@resumePath", resumePath);

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

        public bool StoreResumePathInJobApplicationProfile(string email, string resumePath)
        {
            try
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    string sqlStmt = null;


                    sqlStmt = @"UPDATE [Tbl_Job_Application] set [Resume_Upload] = @resumePath where [Email_Id] = @email";


                    var cmd = new SqlCommand(sqlStmt, cn);

                    // cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar)).Value = email;
                    // cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);

                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@resumePath", resumePath);

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