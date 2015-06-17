using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginLogout.Models
{
    public class User
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name="Email:")]
        [Remote("doesEmailExist", "User")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength=6)]
        [Display(Name = "Lösenord:")]
        public string Password { get; set; }
    }
}