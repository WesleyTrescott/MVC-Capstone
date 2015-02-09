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
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "E-Mail Address")]
        public string Email { get; set; }
        
        [Display(Name = "Remember me on this computer")]
        public bool RememberMe { get; set; }

        public bool IsValid(string _username, string password)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='c:\users\wesley\documents\visual studio 2013\Projects\User Login\User Login\App_Data\Database1.mdf';Integrated Security=True"))
            {
                string _sql = @"SELECT [Username] FROM [dbo].[System_Users] WHERE [Username] = @u AND [Password] = @p";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = _username;
                cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if(reader.HasRows)
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

        public bool RegisterUser(string username, string password, string email)
        {
            if (!IsValid(username, password))
            {
                using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='c:\users\wesley\documents\visual studio 2013\Projects\User Login\User Login\App_Data\Database1.mdf';Integrated Security=True"))
                {
                    string sqlStmt = @"INSERT INTO [dbo].[System_Users] (Username, Password, Email) VALUES (@u, @p, @e)"; 
                    var command = new SqlCommand(sqlStmt, cn);
                    command.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = username;
                    command.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = Helpers.SHA1.Encode(password);
                    command.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar)).Value = email;
                    cn.Open();
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                return true;
            }
            else
                return false;
        }
    }
}