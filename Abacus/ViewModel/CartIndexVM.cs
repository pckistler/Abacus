using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abacus.ViewModel
{
    public class CartIndexVM
    {
        public IEnumerable<Abacus.Models.Cart> Carts { get; set; }
        public System.Web.Mvc.SelectList SearchOptions { get; set; }
    }
}