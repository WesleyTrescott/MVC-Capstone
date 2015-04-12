using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace User_Login.Models
{
    public class ContactModel
    {
        [Required]
        public string firstname { get; set; }

        [Required]
        public string lastname { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string message { get; set; }
    }
}