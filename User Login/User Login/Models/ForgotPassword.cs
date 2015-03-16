using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class ForgotPassword
    {
        [Required]
        public string EmailId { get; set; }

        public string Guid { get; set; }

        public bool updateGuid(string email, string guid)
        {
            try
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    //user has verified the email. Update Is_Active to 1
                    string sqlStmt = @"UPDATE [Tbl_Users] set [User_Guid] = @guid where [Email_Id] = @email";

                    var cmd = new SqlCommand(sqlStmt, cn);
                    //command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar)).Value = email;
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@guid", guid);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
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