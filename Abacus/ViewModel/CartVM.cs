using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class CartVM
    {
        [Key]
        public int Id { get; set; } = 0;

        [Range(0, int.MaxValue)]
        [Display(Name = "Cart")]
        public int CartNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Sale Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Number of Items")]
        public int NumberOfItems { get; set; }

        [Display(Name = "Number of Sellers")]
        public int NumberOfSellers { get; set; }

        public int ExistingBuyerId { get; set; }
        [ForeignKey("ExistingBuyerId")]
        public virtual Models.UserRecord Buyer { get; set; }

        [Display(Name = "Total Amout in Cart")]
        [DataType(DataType.Currency)]
        public double CartAmount { get; set; }
        [Display(Name = "Total Value of Items")]
        [DataType(DataType.Currency)]
        public double ItemsAmount { get; set; }
        [Display(Name = "Total Shipping Costs")]
        [DataType(DataType.Currency)]
        public double ShippingAmount { get; set; }
        [Display(Name = "Total PayPal Fees")]
        [DataType(DataType.Currency)]
        public double PayPalAmount { get; set; }

        [Display(Name ="Seller")]
        public int SellerId { get; set; }
        [ForeignKey("SellerId")]
        public virtual Models.UserRecord Seller { get; set; }
        [Display(Name = "Total Items Cost")]
        [DataType(DataType.Currency)]
        public double SellerItemsTotal { get; set; }
        [Display(Name = "Total Shipping Costs")]
        [DataType(DataType.Currency)]
        public double SellerShippingTotal { get; set; }

        public string TrackingNumber { get; set; }
    }
}