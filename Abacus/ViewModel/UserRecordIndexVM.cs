using Abacus.Models;
using System.Collections.Generic;

namespace Abacus.ViewModel
{
    public class UserRecordIndexVM
    {
        public IEnumerable<UserRecord> UserRecords { get; set; }

        public System.Web.Mvc.SelectList SearchOptions { get; set; }

    }
}