using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class HobbyDBUser
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "HobbyDB User Name")]
        public string HobbyDBUserName { get; set; }

        [Display(Name = "HobbyDB User Id")]
        public int HobbyDBUserId { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name ="PayPal Email")]
        [DataType(DataType.EmailAddress)]
        public string PayPalEmail { get; set; }

        [Display(Name = "Buyer")]
        public bool IsBuyer { get; set; }

        [Display(Name = "Seller")]
        public bool IsSeller{ get; set; }

        public bool IsNewRecord { get; set; }
    }
}