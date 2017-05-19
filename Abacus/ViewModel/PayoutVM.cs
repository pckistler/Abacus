using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class PayoutVM
    {
        public PayoutVM()
        {
            Dialog = new DialogData();
            Date = DateTime.Now;
        }

        public PayoutVM(Models.Payout payout) : this()
        {
            Id = payout.Id;
            UserId = payout.SellerId;
            Date = payout.Date;
            Amount = payout.Amount;            
        }

        public int Id { get; set; }
        public int UserId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        public double Amount { get; set; }

        public double BalanceDue { get; set; }

        public DialogData Dialog { get; set; }
        public class DialogData
        {
            public string UpdateTarget { get; set; }
        }
    }
}