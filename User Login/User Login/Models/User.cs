using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class User
    {
        //[Required]
        //[Display(Name = "Username")]
        //public string UserName { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(20, ErrorMessage = "The {0} must be {2} - {1} characters long", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage="Passwords do not match! Try again!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")] 
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "E-Mail Address")]
        public string Email { get; set; }
        
        [Display(Name = "Remember me on this computer")]
        public bool RememberMe { get; set; }

        //public bool IsValid(string _username, string password)
        public bool IsValid(string email, string password)
        {
            try
            {
                //using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='c:\users\wesley\documents\visual studio 2013\Projects\User Login\User Login\App_Data\Database1.mdf';Integrated Security=True"))
                // using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Login.mdf';Integrated Security=True"))
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    string _sql = @"SELECT [Email_Id] FROM [dbo].[Tbl_Users] WHERE [Email_Id] = @u AND [Password] = @p";
                    //    string _sql = @"SELECT [EmailId] 
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
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RegisterUser(string firstName, string lastName, string password, string email, string guid)
        {
            try
            {
                if (!IsValid(email, password))
                {
                    using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                    {
                        //store Is_Active = 0 until user has verified the email
                        //store global unique id (guid) in database which will used to verify the email
                        string sqlStmt = @"INSERT INTO [DBO].[Tbl_Users] (User_First_Name, User_Last_Name, Email_Id, Password, Is_Active, User_Guid) values (@firstName, @lastName, @email, @password,0, @guid)";
                        var command = new SqlCommand(sqlStmt, cn);

                        command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);
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
            catch(Exception ex)
            {
                return false;
            }
        }
 
        //user has confirmed the email
        public bool Confirmed(string email)
        {
            try 
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                {
                    //user has verified the email. Update Is_Active to 1
                    string sqlStmt = @"UPDATE [Tbl_Users] set [Is_Active] = 1 where [Email_Id] = @email";
                    
                    var command = new SqlCommand(sqlStmt, cn);
                    command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar)).Value = email;
                    
                    cn.Open();
                    command.ExecuteNonQuery();
                    command.Dispose();
                    return true;
                }   
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public bool DeleteUser(string email)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
            {
                string sqlStmt1 = @"DELETE FROM [Tbl_Job_Application] where [Email_Id] = @email";
                string sqlStmt2 = @"DELETE FROM [Tbl_Users] where [Email_Id] = @email";
                var cmd = new SqlCommand(sqlStmt1, cn);

                cmd.Parameters.AddWithValue("@email", email);
                cn.Open();
                int rows = cmd.ExecuteNonQuery();
                cmd.Dispose();

                var cmd2 = new SqlCommand(sqlStmt2, cn);

                cmd2.Parameters.AddWithValue("@email", email);
                cmd2.ExecuteNonQuery();
                cmd2.Dispose();  
                return true;
            }
        }

    }
}