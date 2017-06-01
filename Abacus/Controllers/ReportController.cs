using Abacus.Models;
using Abacus.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abacus.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin,User")]
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Report
        public ActionResult Index()
        {
            ReportVM report = new ReportVM();

            report.Carts = db.Carts.ToList();
            report.Payouts = db.PayOuts.ToList();
            report.Transactions = db.TransactionRecords.ToList();

            return View(report);
        }
    }
}