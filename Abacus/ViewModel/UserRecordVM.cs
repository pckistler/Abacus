using Abacus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class UserRecordVM
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public UserRecordVM(UserRecord userRecord)
        {
            Id = userRecord.Id;
            FirstName = userRecord.FirstName;
            LastName = userRecord.LastName;
            hobbyDBUserName = userRecord.HDBUserName;
            Notes = userRecord.Notes;
            Payouts = userRecord.Payouts;
            Carts = db.Carts.Where(c => c.Transactions.Any(t => t.SellerId == Id)).ToList();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string hobbyDBUserName { get; set; }
        public string Notes { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Payout> Payouts { get; set; }

    }
}