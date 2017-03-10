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

        public double Payout
        {
            get
            {
                double paypal = 0.3 + .029 * (ItemCosts + ShippingCost);
                return ItemCosts - (.07 * ItemCosts + paypal) + ShippingCost;
            }
        }
    }
}