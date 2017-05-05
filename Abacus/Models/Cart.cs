using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Abacus.Models
{
    [Guid("9550DBC5-D187-4EEC-9FF0-26198EFEC593")]
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name ="Cart")]
        public int CartNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Sale Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }

        [Display(Name ="Number of Items")]
        public int NumberOfItems { get; set; }

        [Display(Name = "Number of Sellers")]
        public int NumberOfSellers { get; set; }

        public int BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public virtual UserRecord Buyer { get; set; }

        public int BuyerEmailId { get; set; }
        [ForeignKey("BuyerEmailId")]
        public virtual Email BuyerEmail { get; set; }

        [Display(Name = "Overall value")]
        [DataType(DataType.Currency)]
        public double TotalValue { get; set; }
        [Display(Name = "Total Value of Items")]
        public double ItemCost { get; set; }
        [Display(Name = "Total Shipping Costs")]
        public double ShippingCost { get; set; }
        [Display(Name = "Total PayPal Fees")]
        public double PayPalFees { get; set; }

        public double HobbyDBFees { get; set; }
        //{
        //    get { return ItemCost * .07 + PayPalFees; }
        //}
        public virtual ICollection<TransactionRecord> Transactions { get; set; }

        public void ComputeFees(int numSellers)
        {
            PayPalFees = 0.30 * numSellers + .029 * (ItemCost + ShippingCost);
            HobbyDBFees = ItemCost * .07 + PayPalFees;
        }

        public enum SearchOptions
        {
            CartNumber,
            SellerName,
            SellerUsername,
            BuyerName,
            BuyerUsername,
            BuyerEmail,
            Date
        }

        public static Dictionary<SearchOptions, string> SearchOptionNames = new Dictionary<SearchOptions, string>()
        {
            { SearchOptions.BuyerEmail, "Buyer email" },
            { SearchOptions.BuyerName, "Buyer name" },
            { SearchOptions.BuyerUsername, "Buyer username" },
            { SearchOptions.CartNumber, "Cart number" },
            { SearchOptions.Date, "Date" },
            { SearchOptions.SellerName, "Seller name" },
            { SearchOptions.SellerUsername, "Seller username" }
        };
    }
}