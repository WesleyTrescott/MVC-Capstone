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

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(20, ErrorMessage = "The {0} must be {2} - {1} characters long", MinimumLength = 6)]
        public string Password { get; set; }        //new password

        [Compare("Password", ErrorMessage = "Passwords do not match! Try again!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public bool UpdatePassword(string email, string currentPassword, string password, string guid)
        {
            try
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    string sqlStmt = null;

                    //update password
                    if (currentPassword == null && !System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                        //forgot password
                        sqlStmt = @"UPDATE [Tbl_Users] set [Password] = @password, [User_Guid] = @guid where [Email_Id] = @email";
                    else
                        //change password after logging in
                        sqlStmt = @"UPDATE [Tbl_Users] set [Password] = @password, [User_Guid] = @guid where [Email_Id] = @email and [Password] = @currentPassword";

                    var cmd = new SqlCommand(sqlStmt, cn);

                   // cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar)).Value = email;
                   // cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);

                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", Helpers.SHA1.Encode(password));
                    cmd.Parameters.AddWithValue("@currentPassword", Helpers.SHA1.Encode(currentPassword));
                    cmd.Parameters.AddWithValue("@guid", guid);
                    
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