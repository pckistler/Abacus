using Abacus.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class ManageUsersListVM
    {
        public List<UserVM> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }

    public class UserVM
    {
        public string Id { get; set; }

        [Display(Name="User name")]
        public string UserName { get; set; }

        [Display(Name ="Role")]
        public string Role { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}