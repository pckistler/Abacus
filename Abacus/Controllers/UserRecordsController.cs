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
