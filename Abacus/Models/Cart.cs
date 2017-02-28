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

        [Display(Name = "Total Amout in Cart")]
        [DataType(DataType.Currency)]
        public double TotalValue { get; set; }
        [Display(Name = "Total Value of Items")]
        public double ItemCost { get; set; }
        [Display(Name = "Total Shipping Costs")]
        public double ShippingCost { get; set; }
        [Display(Name = "Total PayPal Fees")]
        public double PayPalFees { get; set; }

        public virtual ICollection<TransactionRecord> Transactions { get; set; }
    }
}