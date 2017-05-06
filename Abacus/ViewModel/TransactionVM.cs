using Abacus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class TransactionVM
    {
        public TransactionVM()
        {

        }

        public TransactionVM(TransactionRecord tr)
        {            
            TransactionRecordId = tr.Id;
            SellerId = tr.SellerId;
            SellerName = tr.Seller.HDBUserName;
            ItemsTotal = tr.ItemCosts;
            ShippingTotal = tr.ShippingCost;
            TrackingNumber = tr.ShippingRecord.TrackingNumber;
            NumItems = tr.NumOfItems;
        }

        public TransactionRecord TransactionRecord
        {
            get
            {
                TransactionRecord rec = new TransactionRecord()
                {
                    Id = TransactionRecordId,
                    NumOfItems = NumItems,
                    SellerId = SellerId,
                    ItemCosts = ItemsTotal,
                    ShippingCost = ShippingTotal                     
                };
                rec.ShippingRecord = new ShippingRecord() { TrackingNumber = TrackingNumber };

                return rec;
            }
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public int TransactionRecordId { get; set; }

        [Display(Name = "Seller")]
        public int SellerId { get; set; }
        public string SellerName { get; set; }

        public System.Web.Mvc.SelectList Sellers { get; set; }

        [RegularExpression(@"[1-9][0-9]*", ErrorMessage = "Must be a whole number")]
        [Display(Name = "Number of items")]
        public int NumItems { get; set; }

        [Display(Name = "Total Items Cost")]
        [DataType(DataType.Currency)]
        public double ItemsTotal { get; set; }

        [Display(Name = "Total Shipping Costs")]
        [DataType(DataType.Currency)]
        public double ShippingTotal { get; set; }

        [Display(Name = "Tracking number")]
        public string TrackingNumber { get; set; }

        public string UpdateTargetId { get; set; } = "cart_transaction_list";

        public static List<TransactionVM> Transactions(IEnumerable<TransactionRecord> records)
        {
            List<TransactionVM> list = new List<TransactionVM>();
            foreach (var item in records)
            {
                list.Add(new TransactionVM(item));
            }
            return list;
        }
    }


}