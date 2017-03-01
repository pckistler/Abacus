using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class HobbyDBUser
    {
        public HobbyDBUser()
        {                
        }

        public HobbyDBUser(Models.UserRecord user)
        {
            Id = user.Id;
            HobbyDBUserName = user.HDBUserName;
            HobbyDBUserId = user.HDBUserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.PreferredEmail.EmailAddress;
            PayPalEmail = user.PayPalId.EmailAddress;
            IsBuyer = (user.UserType & Models.UserRecord.UserTypes.Buyer) == Models.UserRecord.UserTypes.Buyer;
            IsSeller = (user.UserType & Models.UserRecord.UserTypes.Seller) == Models.UserRecord.UserTypes.Seller;
            IsNewRecord = false;
        }

        public Models.UserRecord UserRecord
        {
            get
            {
                Models.UserRecord ur = new Models.UserRecord()
                {
                    Id = Id,
                    HDBUserId =HobbyDBUserId,
                    HDBUserName = HobbyDBUserName,
                    FirstName = FirstName,
                    LastName = LastName,                    
                };
                ur.UserType = (IsBuyer ? Models.UserRecord.UserTypes.Buyer : 0) | (IsSeller ? Models.UserRecord.UserTypes.Seller : 0);
                return ur;
            }
        }

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

        [Display(Name ="First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Buyer")]
        public bool IsBuyer { get; set; }

        [Display(Name = "Seller")]
        public bool IsSeller{ get; set; }

        public bool IsNewRecord { get; set; }
    }
}