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

        //public bool RegisterUser(string username, string password, string email)
        public bool RegisterUser(string firstName, string lastName, string password, string email)
        {
            try
            {
                //if (!IsValid(username, password))
                if (!IsValid(email, password))
                {
                    //using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='c:\users\wesley\documents\visual studio 2013\Projects\User Login\User Login\App_Data\Database1.mdf';Integrated Security=True"))
                    //using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Login.mdf';Integrated Security=True"))
                    using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
                    {
                        //string sqlStmt = @"INSERT INTO [dbo].[Tbl_User_Login] (Username, Password, Email) VALUES (@u, @p, @e)"; 
                        string sqlStmt = @"INSERT INTO [DBO].[Tbl_Users] (User_First_Name, User_Last_Name, Email_Id, Password) values (@firstName, @lastName, @email, @password)";
                        var command = new SqlCommand(sqlStmt, cn);
                        //command.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = username;
                        command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);
                        command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar)).Value = email;
                        command.Parameters.Add(new SqlParameter("@firstName", SqlDbType.NVarChar)).Value = firstName;
                        command.Parameters.Add(new SqlParameter("@lastName", SqlDbType.NVarChar)).Value = lastName;
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
    }
}