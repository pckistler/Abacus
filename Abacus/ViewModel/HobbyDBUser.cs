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

        [Required]
        [Display(Name = "HobbyDB User Name")]
        public string HobbyDBUserName { get; set; }

        [Required]
        [Range(1,int.MaxValue)]
        [Display(Name = "HobbyDB User Id")]
        public int HobbyDBUserId { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
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