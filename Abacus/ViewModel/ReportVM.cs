using Abacus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class ReportVM
    {
        public IEnumerable<Cart> Carts { get; set; }
        public IEnumerable<Payout> Payouts { get; set; }

        public IList<TransactionRecord> Transactions { get; set; }
    }
}