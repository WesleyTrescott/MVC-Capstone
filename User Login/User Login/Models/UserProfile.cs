﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using User_Login.Models;

namespace User_Login.Models
{
    public class UserProfile
    {
        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
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

        [Display(Name = "Experience_Years")]
        public int? Experience_Years { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public int? phone_number { get; set; }

        //public bool UpdateProfile(string email, string firstName, string lastName, string street, string city, string state, string country, string phone_number, int? experience_years ,string skills)
        public bool UpdateProfile(string email, string firstName, string lastName, string street, string city, string state, string country, int? phone_number, int? experience_years, string skills)    
    {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Job_Candidate_Application.mdf';Integrated Security=True"))
            {
                string sqlStmt = @"UPDATE [Tbl_Users] set [User_First_Name] = @firstName, [User_Last_Name] = @lastName, [User_Street] = @street, [User_City] = @city, [User_State] = @state, [User_Country] = @country, [User_Phone_Number]=@phone_number, [Exp_Years]=@experience, [Skills]=@skills where [Email_Id] = @email";
                var command = new SqlCommand(sqlStmt, cn);

                command.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar)).Value = email;
                command.Parameters.Add(new SqlParameter("@firstName", SqlDbType.VarChar)).Value = firstName;
                command.Parameters.Add(new SqlParameter("@lastName", SqlDbType.VarChar)).Value = lastName;
                command.Parameters.Add(new SqlParameter("@street", SqlDbType.VarChar)).Value = street;
                command.Parameters.Add(new SqlParameter("@city", SqlDbType.VarChar)).Value = city;
                command.Parameters.Add(new SqlParameter("@state", SqlDbType.VarChar)).Value = state;
                command.Parameters.Add(new SqlParameter("@country", SqlDbType.VarChar)).Value = country;

                command.Parameters.Add(new SqlParameter("@phone_number", SqlDbType.Int));
                if (phone_number != null)
                {
                    command.Parameters["@phone_number"].Value = phone_number;
                }
                else
                {
                    command.Parameters["@phone_number"].Value = DBNull.Value;
                }

                command.Parameters.Add(new SqlParameter("@skills", SqlDbType.VarChar));
                if (skills != null)
                {
                    command.Parameters["@skills"].Value = skills;
                }
                else
                {
                    command.Parameters["@skills"].Value = DBNull.Value;
                }

                command.Parameters.Add(new SqlParameter("@experience", SqlDbType.Int));
                if (experience_years != null)
                {
                    command.Parameters["@experience"].Value = experience_years;
                }
                else
                {
                    command.Parameters["@experience"].Value = DBNull.Value;
                }

                cn.Open();
                command.ExecuteNonQuery();
                command.Dispose();
            }
            return true;
        }
    }
}