//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace User_Login.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Customer
    {
        public int Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_Contact_Name { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Contact_Id { get; set; }
        public string Customer_Job_Id { get; set; }
    
        public virtual Tbl_Customer Tbl_Customer1 { get; set; }
        public virtual Tbl_Customer Tbl_Customer2 { get; set; }
    }
}