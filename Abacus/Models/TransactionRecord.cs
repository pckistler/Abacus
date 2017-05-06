using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.Models
{
    public class TransactionRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }

        public int SellerId { get; set; }
        [ForeignKey("SellerId")]
        public virtual UserRecord Seller { get; set; }

        public double ItemCosts { get; set; }
        public double ShippingCost { get; set; }
        public double Fees { get; set; }
        public int ShippingRecordId { get; set; }
        [ForeignKey("ShippingRecordId")]
        public virtual ShippingRecord ShippingRecord { get; set; }

        public double PayPalFees { get; set; }
        public double HobbyDBFees { get; set; }
        public double PayOut { get; set; }

        public int NumOfItems { get; set; }

        public void ComputeFees()
        {
            PayPalFees = 0.3 + .029 * (ItemCosts + ShippingCost);
            HobbyDBFees = .07 * ItemCosts + PayPalFees;
            PayOut = ItemCosts - (HobbyDBFees) + ShippingCost;
        }
    }
}