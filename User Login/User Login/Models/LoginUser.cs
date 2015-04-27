using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class LoginUser
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[StringLength(20, ErrorMessage = "The {0} must be {2} - {1} characters long", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "E-Mail Address")]
        public string Email { get; set; }

        [Display(Name = "Remember me on this computer")]
        public bool RememberMe { get; set; }

        public bool IsValid(string email, string password)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
            {
                // user needs to verify email before they can log in. 
                // Verifying email changes Is_Active to 1
                string _sql = @"SELECT [Email_Id] FROM [Tbl_Users] WHERE [Email_Id] = @u AND [Password] = @p AND [Is_Active] = 1";
                var cmd = new SqlCommand(_sql, cn);

                cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = email;
                cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);

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

        public bool IsAdmin(string email, string password)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
            {
                string _sql = @"SELECT [Admin_Email_Id] FROM [Tbl_Admin] WHERE [Admin_Email_Id] = @u AND [Admin_Password] = @p";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = email;
                cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);
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

        public bool isGoogleUserRegistered(string email)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
            {
                string _sql = @"SELECT [Email_Id] FROM [Tbl_Users] WHERE [Email_Id] = @u AND [Is_Active] = 1";
                var cmd = new SqlCommand(_sql, cn);

                cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = email;

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

        public bool RegisterGoogleUser(string firstName, string lastName, string email, string guid)
        {
            try
            {
                if (!isGoogleUserRegistered(email))
                {
                    using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                    {
                        //store Is_Active = 0 until user has verified the email
                        //store global unique id (guid) in database which will used to verify the email
                        string sqlStmt = @"INSERT INTO [DBO].[Tbl_Users] (User_First_Name, User_Last_Name, Email_Id, Is_Active, User_Guid) values (@firstName, @lastName, @email, 0, @guid)";
                        var command = new SqlCommand(sqlStmt, cn);

                        command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar)).Value = email;
                        command.Parameters.Add(new SqlParameter("@firstName", SqlDbType.NVarChar)).Value = firstName;
                        command.Parameters.Add(new SqlParameter("@lastName", SqlDbType.NVarChar)).Value = lastName;
                        command.Parameters.Add(new SqlParameter("@guid", SqlDbType.NVarChar)).Value = guid;

                        cn.Open();
                        command.ExecuteNonQuery();
                        command.Dispose();
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}