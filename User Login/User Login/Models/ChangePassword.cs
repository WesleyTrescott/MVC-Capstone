using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class ChangePassword
    {
        public string EmailId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(20, ErrorMessage = "The {0} must be {2} - {1} characters long", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match! Try again!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public bool UpdatePassword(string email, string password)
        {
            try
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    //user has verified the email. Update Is_Active to 1
                    string sqlStmt = @"UPDATE [Tbl_Users] set [Password] = @password where [Email_Id] = @email";

                    var cmd = new SqlCommand(sqlStmt, cn);

                    cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar)).Value = email;
                    cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);
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
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}