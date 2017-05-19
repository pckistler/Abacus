using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abacus.Models;
using Abacus.ViewModel;

namespace Abacus.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin,User")]
    public class UserRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserRecords
        public async Task<ActionResult> Index()
        {
            var userRecords = db.UserRecords.OrderBy(u=>u.HDBUserName).Include(u => u.PayPalId).Include(u => u.PreferredEmail);
            return View(await userRecords.ToListAsync());
        }

        // GET: UserRecords/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRecord userRecord = await db.UserRecords.FindAsync(id);
            if (userRecord == null)
            {
                return HttpNotFound();
            }
            return View(new Abacus.ViewModel.UserRecordVM(userRecord));
        }

        // GET: UserRecords/Create
        public ActionResult Create()
        {
            ViewBag.PayPalEmailId = new SelectList(db.Emails, "Id", "EmailAddress");
            //ViewBag.PreferredAddressId = new SelectList(db.Addresses, "Id", "Street1");
            ViewBag.PreferredEmailId = new SelectList(db.Emails, "Id", "EmailAddress");
            return View();
        }

        // POST: UserRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HDBUserName,HDBUserId,FirstName,LastName,PayPalEmailId,PreferredEmailId,PreferredAddressId")] UserRecord userRecord)
        {
            if (ModelState.IsValid)
            {
                db.UserRecords.Add(userRecord);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PayPalEmailId = new SelectList(db.Emails, "Id", "EmailAddress", userRecord.PayPalEmailId);
            //ViewBag.PreferredAddressId = new SelectList(db.Addresses, "Id", "Street1", userRecord.PreferredAddressId);
            ViewBag.PreferredEmailId = new SelectList(db.Emails, "Id", "EmailAddress", userRecord.PreferredEmailId);
            return View(userRecord);
        }

        // GET: UserRecords/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRecord userRecord = await db.UserRecords.FindAsync(id);
            if (userRecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.PayPalEmailId = new SelectList(db.Emails, "Id", "EmailAddress", userRecord.PayPalEmailId);
            //ViewBag.PreferredAddressId = new SelectList(db.Addresses, "Id", "Street1", userRecord.PreferredAddressId);
            ViewBag.PreferredEmailId = new SelectList(db.Emails, "Id", "EmailAddress", userRecord.PreferredEmailId);
            return View(userRecord);
        }

        // POST: UserRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HDBUserName,HDBUserId,FirstName,LastName,PayPalEmailId,PreferredEmailId,PreferredAddressId")] UserRecord userRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userRecord).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PayPalEmailId = new SelectList(db.Emails, "Id", "EmailAddress", userRecord.PayPalEmailId);
            //ViewBag.PreferredAddressId = new SelectList(db.Addresses, "Id", "Street1", userRecord.PreferredAddressId);
            ViewBag.PreferredEmailId = new SelectList(db.Emails, "Id", "EmailAddress", userRecord.PreferredEmailId);
            return View(userRecord);
        }

        // GET: UserRecords/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRecord userRecord = await db.UserRecords.FindAsync(id);
            if (userRecord == null)
            {
                return HttpNotFound();
            }
            return View(userRecord);
        }

        // POST: UserRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserRecord userRecord = await db.UserRecords.FindAsync(id);
            db.UserRecords.Remove(userRecord);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult UserDetails(int Id)
        {
            var user = db.UserRecords.FirstOrDefault(u => u.Id == Id);
            if (user != null)
            {
                return PartialView("_Details", user);
            }
            return View();
        }
        public ActionResult UserCarts(int Id)
        {
            var items = db.Carts.Where(c=>c.Transactions.Any(u=>u.SellerId == Id)).OrderByDescending(c=>c.SaleDate).ToList();
            if (items != null)
            {
                if (!ViewData.ContainsKey("UserId"))
                    ViewData.Add("UserId", Id);
                ViewData["UserId"] = Id;
                ActionResult ar = PartialView("_Carts", items);
                //ViewData.Remove("UserId");
                return ar;
            }
            return View();
        }


        public ActionResult NotesDlgContent(int Id)
        {
            var user = db.UserRecords.FirstOrDefault(u => u.Id == Id);
            if (user == null)
                return View();

            return PartialView("_NotesDlg", user);
        }


        [HttpPost]
        public ActionResult Notes(UserRecord ur)
        {
            var user = db.UserRecords.FirstOrDefault(u => u.Id == ur.Id);
            if (user == null)
                return View();
            user.Notes = ur.Notes;
            db.SaveChanges();
            return new ContentResult() { Content = ur.Notes };
        }
        
        public ActionResult PayoutAmount(int id)
        {
            var items = db.PayOuts.Where(c => c.SellerId == id).OrderByDescending(c => c.Date).ToList();
            if (items == null)
                return new ContentResult() { Content = "$0.00" };
            return new ContentResult() { Content = string.Format("$ {0:0.00}",items.Sum(p=>p.Amount)) };
        }

        public ActionResult UserPayouts(int Id)
        {
            var items = db.PayOuts.Where(c => c.SellerId == Id).OrderByDescending(c => c.Date).ToList();
            if (items != null)
            {
                return PartialView("_Payout", items);
            }
            return View();
        }
        public ActionResult PayoutDlgContent(int Id)
        {
            PayoutVM payoutVM = new PayoutVM();
            payoutVM.UserId = Int32.Parse(Request.Params["UserId"]);

            if (payoutVM.UserId > 0)
            {
                var payouts = db.PayOuts.Where(c => c.SellerId == payoutVM.UserId);
                var sales = db.TransactionRecords.Where(t => t.SellerId == payoutVM.UserId);
                double payoutAmount = payouts.ToList().Sum(p => p.Amount);
                double salesAmount = sales.ToList().Sum(t => t.PayOut);
                payoutVM.Amount = salesAmount - payoutAmount;
            }

            var payout = db.PayOuts.FirstOrDefault(c => c.Id == Id);
            if (payout != null)
                payoutVM = new PayoutVM(payout);

            payoutVM.Dialog.UpdateTarget = Request.Params["UpdateTarget"];

            return PartialView("_PayoutDlg", payoutVM);
        }

        [HttpPost]
        public ActionResult Payout(PayoutVM payoutVM)
        {
            if (!ModelState.IsValid)
                return View();
            if (payoutVM.Id == 0)
            {
                Payout payout = new Models.Payout()
                {
                    Date = payoutVM.Date,
                    Amount = payoutVM.Amount,
                    SellerId = payoutVM.UserId
                };
                db.PayOuts.Add(payout);
                db.SaveChanges();

                var items = db.PayOuts.Where(c => c.SellerId == payoutVM.UserId).OrderByDescending(c => c.Date).ToList();
                return PartialView("_Payout", items);
            }
            else
            {
                Payout payout = db.PayOuts.FirstOrDefault(p => p.Id == payoutVM.Id);
                if (payout != null)
                {
                    payout.Date = payoutVM.Date;
                    payout.Amount = payoutVM.Amount;
                    db.SaveChanges();
                    return PartialView("_PayoutListItem", payout);
                }
            }
            return View();
        }

        public ActionResult DeletePayoutDlgContent(int Id)
        {
            var payout = db.PayOuts.FirstOrDefault(c => c.Id == Id);
            PayoutVM payoutVM = new PayoutVM();
            if (payout != null)
                payoutVM = new PayoutVM(payout);

            payoutVM.UserId = Int32.Parse(Request.Params["UserId"]);
            payoutVM.Dialog.UpdateTarget = Request.Params["UpdateTarget"];
            return PartialView("_DeletePayoutDlg", payoutVM);
        }

        [HttpPost]
        public ActionResult DeletePayout(PayoutVM payoutVM)
        {
            if (!ModelState.IsValid)
                return View();
            if (payoutVM.Id != 0)
            {
                Payout payout = db.PayOuts.FirstOrDefault(p => p.Id == payoutVM.Id);
                if (payout != null)
                {
                    db.PayOuts.Remove(payout);
                    db.SaveChanges();
                    return new ContentResult() { Content = string.Format("PayOut{0}",payout.Id) };// PartialView("_PayoutListItem", payout);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult HobbyDBUser(HobbyDBUserVM hdbUser)
        {
            int userRecordId = 0;
            if (ModelState.IsValid)
            {
                bool error = false;
                if (string.IsNullOrEmpty(hdbUser.HobbyDBUserName) || hdbUser.HobbyDBUserId <= 0)
                {
                    ModelState.AddModelError(nameof(hdbUser.HobbyDBUserName), "The specified UserName is not valid.");
                    error = false;
                }
                if (hdbUser.HobbyDBUserId <= 0)
                {
                    ModelState.AddModelError(nameof(hdbUser.HobbyDBUserId), "The specified User Id is not valid.");
                    error = false;
                }
                if (string.IsNullOrEmpty(hdbUser.Email))
                {
                    ModelState.AddModelError(nameof(hdbUser.Email), "The specified Email is not valid.");
                    error = false;
                }
                if (!hdbUser.IsBuyer && !hdbUser.IsSeller)
                {
                    ModelState.AddModelError(nameof(hdbUser.IsBuyer), "Buyer or Seller must be checked.");
                    error = false;
                }
                if (error)
                    return PartialView("_HobbyDBUser", hdbUser);

                UserRecord user = db.UserRecords.Where(u=>u.Id != hdbUser.Id).SingleOrDefault(u => u.HDBUserId == hdbUser.HobbyDBUserId || u.HDBUserName.Equals(hdbUser.HobbyDBUserName, StringComparison.CurrentCultureIgnoreCase));
                if (user != null)
                {
                    string ctrl = user.HDBUserId == hdbUser.HobbyDBUserId ? nameof(hdbUser.HobbyDBUserId) : nameof(hdbUser.HobbyDBUserName);
                    if (user.HDBUserId == hdbUser.HobbyDBUserId)
                    {
                        ModelState.AddModelError(nameof(hdbUser.HobbyDBUserId), "User with the specified User Id already exists");
                    }
                    if (string.Compare(user.HDBUserName, hdbUser.HobbyDBUserName, true) == 0)
                    {
                        ModelState.AddModelError(nameof(hdbUser.HobbyDBUserName), "User with the specified User Id already exists");
                    }
                    return PartialView("_HobbyDBUser", hdbUser);
                }

                if (error)
                    return PartialView("_HobbyDBUser", hdbUser);

                Email preferredEmail = db.Emails.SingleOrDefault(e => string.Compare(e.EmailAddress, hdbUser.Email, true) == 0);
                if (preferredEmail == null)
                {
                    preferredEmail = new Email() { EmailAddress = hdbUser.Email };
                    db.Emails.Add(preferredEmail);
                    db.SaveChanges();
                }
                Email payPalEmail = db.Emails.SingleOrDefault(e => string.Compare(e.EmailAddress, hdbUser.PayPalEmail, true) == 0);
                if (payPalEmail == null)
                {
                    payPalEmail = new Email() { EmailAddress = hdbUser.PayPalEmail };
                    db.Emails.Add(payPalEmail);
                    db.SaveChanges();
                }

                if (hdbUser.Id == 0)
                {


                    UserRecord ur = new UserRecord()
                    {
                        HDBUserName = hdbUser.HobbyDBUserName,
                        HDBUserId = hdbUser.HobbyDBUserId,
                        LastName = hdbUser.LastName,
                        FirstName = hdbUser.FirstName,
                        PhoneNumber = hdbUser.PhoneNumber,
                        UserType = (hdbUser.IsBuyer ? UserRecord.UserTypes.Buyer : 0) | (hdbUser.IsSeller ? UserRecord.UserTypes.Seller : UserRecord.UserTypes.None),
                        PreferredEmailId = preferredEmail.Id,
                        PayPalEmailId = payPalEmail.Id
                    };
                    db.UserRecords.Add(ur);
                    db.SaveChanges();
                    userRecordId = ur.Id;

                    var userRecords = db.UserRecords.Include(u => u.PayPalId).Include(u => u.PreferredEmail);
                    return PartialView("_UserList", userRecords.ToList());
                }
                else
                {
                    UserRecord ur = db.UserRecords.First(x=>x.Id == hdbUser.Id);
                    ur.HDBUserName = hdbUser.HobbyDBUserName;
                    ur.HDBUserId = hdbUser.HobbyDBUserId;
                    ur.FirstName = hdbUser.FirstName;
                    ur.LastName = hdbUser.LastName;
                    ur.PhoneNumber = hdbUser.PhoneNumber;
                    ur.UserType = (hdbUser.IsBuyer ? UserRecord.UserTypes.Buyer : UserRecord.UserTypes.None) | (hdbUser.IsSeller ? UserRecord.UserTypes.Seller : UserRecord.UserTypes.None);
                    ur.PayPalEmailId = payPalEmail.Id;
                    ur.PreferredEmailId = preferredEmail.Id;
                    db.Entry(ur).State = EntityState.Modified;
                    db.SaveChanges();

                    string pv = "_UserListItem";
                    if (!string.IsNullOrEmpty(hdbUser.Dialog.PartialView))
                        pv = hdbUser.Dialog.PartialView;
                    return PartialView(pv, ur);
                }
            }
            else
            {
                return PartialView(hdbUser);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
