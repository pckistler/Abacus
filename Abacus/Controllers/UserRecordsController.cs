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
    public class UserRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserRecords
        public async Task<ActionResult> Index()
        {
            var userRecords = db.UserRecords.Include(u => u.PayPalId).Include(u => u.PreferredEmail);
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
            return View(userRecord);
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



        [HttpPost]
        public ActionResult HobbyDBUser()
        {
            if (ModelState.IsValid)
            {
                int value = 0;
                int id = 0;
                if (Int32.TryParse(Request.Params[nameof(ViewModel.HobbyDBUser.Id)], out value))
                    id = value;

                string userName = Request.Params[nameof(ViewModel.HobbyDBUser.HobbyDBUserName)];

                int userId = 0;
                if (Int32.TryParse(Request.Params[nameof(ViewModel.HobbyDBUser.HobbyDBUserId)], out value))
                    userId = value;
                string email = Request.Params[nameof(ViewModel.HobbyDBUser.Email)];
                string parsedPayPalEmail = Request.Params[nameof(ViewModel.HobbyDBUser.PayPalEmail)];

                UserRecord.UserTypes buyer = UserRecord.UserTypes.None;
                if (Request.Params[nameof(ViewModel.HobbyDBUser.IsBuyer)].Contains("true"))
                    buyer = UserRecord.UserTypes.Buyer;

                UserRecord.UserTypes seller = UserRecord.UserTypes.None;
                if (Request.Params[nameof(ViewModel.HobbyDBUser.IsSeller)].Contains("true"))
                    seller = UserRecord.UserTypes.Seller;

                UserRecord user = db.UserRecords.SingleOrDefault(u => u.HDBUserId == userId || string.Compare(u.HDBUserName, userName, true) == 0);
                if (user != null)
                {
                    ModelState.AddModelError("", "User with the specified UserName or Id already exists");
                    HobbyDBUser ur = new ViewModel.HobbyDBUser() {
                        IsNewRecord = (id == 0),
                        Id = id,
                        IsBuyer = buyer == UserRecord.UserTypes.Buyer,
                        IsSeller = seller == UserRecord.UserTypes.Seller,
                        HobbyDBUserName = userName,
                        HobbyDBUserId = userId,
                        Email = email,
                        PayPalEmail = parsedPayPalEmail
                    };

                    return PartialView("_HobbyDBUser", ur);
                }

                if (id == 0)
                {
                    bool saveChanges = false;
                    Email preferredEmail = db.Emails.SingleOrDefault(e => string.Compare(e.EmailAddress, email, true) == 0);
                    if (preferredEmail == null)
                    {
                        preferredEmail = new Email() { EmailAddress = email };
                        db.Emails.Add(preferredEmail);
                        saveChanges = true;
                    }
                    Email payPalEmail = db.Emails.SingleOrDefault(e => string.Compare(e.EmailAddress, parsedPayPalEmail, true) == 0);
                    if (payPalEmail == null)
                    {
                        payPalEmail = new Email() { EmailAddress = parsedPayPalEmail };
                        db.Emails.Add(payPalEmail);
                        saveChanges = true;
                    }
                    if (saveChanges)
                        db.SaveChanges();

                    UserRecord ur = new UserRecord()
                    {
                        HDBUserName = userName,
                        HDBUserId = userId,
                        UserType = buyer | seller,
                        PreferredEmailId = preferredEmail.Id,
                        PayPalEmailId = payPalEmail.Id
                    };
                    db.UserRecords.Add(ur);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult ViewHobbyDBUser()
        {
            int value, id = 0;
            if (Int32.TryParse(Request.Params["Id"], out value))
                id = value;
            bool isBuyer = string.Compare(Request.Params["UserType"], "buyer") == 0;            
                
            HobbyDBUser ur = new ViewModel.HobbyDBUser() { IsNewRecord = (id == 0), Id = id, IsBuyer = isBuyer, IsSeller = !isBuyer };

            return PartialView("_HobbyDBUser", ur);
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
