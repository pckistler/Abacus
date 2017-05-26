
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Abacus.Models
{
    [Guid("4C4B9ADF-8C92-4CC9-89F9-01157A4923DA")]
    public class UserRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(64)]
        [Display(Name ="hobbyDB Name")]
        public string HDBUserName { get; set; }
        [Display(Name="hobbyDB Id")]
        public int HDBUserId { get; set; }

        [Display(Name = "First name")]
        [StringLength(64)]
        public string FirstName { get; set; }
        [StringLength(64)]
        [Display(Name = "Last name")]
        public string LastName{ get; set; }

        public int PayPalEmailId { get; set; }
        [Display(Name = "PayPal Email")]
        [ForeignKey("PayPalEmailId")]
        public virtual Email PayPalEmail { get; set; }

        public int PreferredEmailId { get; set; }
        [Display(Name = "Email")]
        [ForeignKey("PreferredEmailId")]
        public virtual Email PreferredEmail { get; set; }

        //public int PreferredAddressId { get; set; }
        //[ForeignKey("PreferredAddressId")]
        //public virtual Address PreferredAddress { get; set; }

        [Display(Name="User type")]
        public UserTypes UserType { get; set; } = UserTypes.None;

        public string PhoneNumber { get; set; }

        public virtual ICollection<Payout> Payouts { get; set; }
        public virtual ICollection<TransactionRecord> Transactions { get; set; }

        public string Notes { get; set; }

        public enum UserTypes
        {
            None = 0,
            Buyer = 1,
            Seller = 2,
            BuyerSeller = Buyer | Seller
        }

        public enum SearchOptions
        {
            None,
            Name,
            Email,
            PayPalEmail,
            PhoneNumber
        }

        public static Dictionary<SearchOptions, string> SearchOptionNames = new Dictionary<SearchOptions, string>()
        {
            { SearchOptions.Name, "Name" },
            { SearchOptions.Email, "Email" },
            { SearchOptions.PayPalEmail, "PayPal email" },
            { SearchOptions.PhoneNumber, "Phone number" },
            { SearchOptions.None, "None" }
        };
    }
}