using Abacus.Models;
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
        public CartVM()
        {

        }

        public CartVM(Models.Cart cart)
        {
            Id = cart.Id;
            CartNumber = cart.CartNumber;
            SaleDate = cart.SaleDate;
            NumberOfItems = cart.NumberOfItems;
            NumberOfSellers = cart.NumberOfSellers;
            BuyerId = cart.BuyerId;
            CartAmount = cart.TotalValue;
            ItemsAmount = cart.ItemCost;
            ShippingAmount = cart.ShippingCost;
            PayPalAmount = cart.PayPalFees;
            Models.TransactionRecord tr = cart.Transactions.ElementAtOrDefault(0);
            TransactionId = tr.Id;
            SellerId = tr.SellerId;
            SellerItemsTotal = tr.ItemCosts;
            SellerShippingTotal = tr.ShippingCost;
            TrackingNumber = string.Empty;
            ShippingRecordId = 0;
            if (tr.ShippingRecord != null)
            {
                ShippingRecordId = tr.Id;
                TrackingNumber = tr.ShippingRecord.TrackingNumber;
            }

            if (cart.Transactions != null && cart.Transactions.Count > 0)
            {
                m_Transactions.AddRange(cart.Transactions);
            }
        }

        private List<TransactionRecord> m_Transactions = new List<TransactionRecord>();
        public Models.Cart Cart
        {
            get
            {
                Models.Cart cart = new Models.Cart()
                {
                    Id = Id,
                    BuyerId = BuyerId,                    
                    CartNumber = CartNumber,
                    TotalValue = CartAmount,
                    ItemCost = ItemsAmount,
                    SaleDate = SaleDate,
                    ShippingCost = ShippingAmount,
                    PayPalFees = PayPalAmount,
                    NumberOfItems = NumberOfItems,
                    NumberOfSellers = NumberOfSellers
                };

                Models.UserRecord buyer = Buyer;
                if (buyer == null)
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    buyer = db.UserRecords.SingleOrDefault(r=>r.Id == BuyerId);
                }
                cart.BuyerEmailId = buyer.PreferredEmailId;

                return cart;
            }
        }
        public List<Models.TransactionRecord> Transactions
        {
            get
            {
                List<Models.TransactionRecord> list = new List<Models.TransactionRecord>();
                Models.TransactionRecord tr = new Models.TransactionRecord()
                {
                    Id = TransactionId,
                    SellerId = SellerId,
                    ItemCosts = SellerItemsTotal,
                    ShippingCost = SellerShippingTotal,
                };
                Models.ShippingRecord sr = new Models.ShippingRecord()
                {
                    Id = ShippingRecordId,
                    TrackingNumber = TrackingNumber
                };
                tr.ShippingRecord = sr;
                tr.ShippingRecordId = sr.Id;
                list.Add(tr);

                return list;
            }
        }
        public List<TransactionVM> m_TransactionVMs = new List<TransactionVM>();
        public List<TransactionVM> TransactionVMs
        {
            set
            {
                m_TransactionVMs = value;
            }
            get
            {
                if (m_TransactionVMs.Count == 0)
                {
                    foreach (var item in m_Transactions)
                    {
                        TransactionVM tvm = new TransactionVM(item);
                        m_TransactionVMs.Add(tvm);
                    }
                }
                return m_TransactionVMs;            
            }
        }

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

        public int BuyerId { get; set; }
        [ForeignKey("BuyerId")]
        public virtual Models.UserRecord Buyer { get; set; }
        public System.Web.Mvc.SelectList Buyers { get; set; }

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
        public System.Web.Mvc.SelectList Sellers { get; set; }

        [Display(Name = "Total Items Cost")]
        [DataType(DataType.Currency)]
        public double SellerItemsTotal { get; set; }
        [Display(Name = "Total Shipping Costs")]
        [DataType(DataType.Currency)]
        public double SellerShippingTotal { get; set; }

        public int TransactionId { get; set; }
        public int ShippingRecordId { get; set; }

        public string TrackingNumber { get; set; }
    }
}