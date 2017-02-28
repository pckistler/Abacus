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
        [Display(Name ="Name")]
        public string HDBUserName { get; set; }
        public int HDBUserId { get; set; }

        [StringLength(64)]
        public string FirstName { get; set; }
        [StringLength(64)]
        public string LastName{ get; set; }

        public int PayPalEmailId { get; set; }
        [ForeignKey("PayPalEmailId")]
        public virtual Email PayPalId { get; set; }

        public int PreferredEmailId { get; set; }
        [ForeignKey("PreferredEmailId")]
        public virtual Email PreferredEmail { get; set; }

        //public int PreferredAddressId { get; set; }
        //[ForeignKey("PreferredAddressId")]
        //public virtual Address PreferredAddress { get; set; }

        public UserTypes UserType { get; set; } = UserTypes.None;

        public enum UserTypes
        {
            None = 0,
            Buyer = 1,
            Seller = 2,
            BuyerSeller = Buyer | Seller
        }
    }
}