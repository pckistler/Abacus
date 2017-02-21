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

namespace Abacus.Controllers
{
    public class TransactionRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TransactionRecords
        public async Task<ActionResult> Index()
        {
            var transactionRecords = db.TransactionRecords.Include(t => t.Cart).Include(t => t.Seller).Include(t => t.ShippingRecord);
            return View(await transactionRecords.ToListAsync());
        }

        // GET: TransactionRecords/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionRecord transactionRecord = await db.TransactionRecords.FindAsync(id);
            if (transactionRecord == null)
            {
                return HttpNotFound();
            }
            return View(transactionRecord);
        }

        // GET: TransactionRecords/Create
        public ActionResult Create()
        {
            ViewBag.CartId = new SelectList(db.Carts, "Id", "Id");
            ViewBag.SellerId = new SelectList(db.UserRecords, "Id", "HDBUserName");
            ViewBag.ShippingRecordId = new SelectList(db.ShippingRecords, "Id", "TrackingNumber");
            return View();
        }

        // POST: TransactionRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CartId,SellerId,ItemCosts,ShippingCost,Fees,ShippingRecordId")] TransactionRecord transactionRecord)
        {
            if (ModelState.IsValid)
            {
                db.TransactionRecords.Add(transactionRecord);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CartId = new SelectList(db.Carts, "Id", "Id", transactionRecord.CartId);
            ViewBag.SellerId = new SelectList(db.UserRecords, "Id", "HDBUserName", transactionRecord.SellerId);
            ViewBag.ShippingRecordId = new SelectList(db.ShippingRecords, "Id", "TrackingNumber", transactionRecord.ShippingRecordId);
            return View(transactionRecord);
        }

        // GET: TransactionRecords/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionRecord transactionRecord = await db.TransactionRecords.FindAsync(id);
            if (transactionRecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.CartId = new SelectList(db.Carts, "Id", "Id", transactionRecord.CartId);
            ViewBag.SellerId = new SelectList(db.UserRecords, "Id", "HDBUserName", transactionRecord.SellerId);
            ViewBag.ShippingRecordId = new SelectList(db.ShippingRecords, "Id", "TrackingNumber", transactionRecord.ShippingRecordId);
            return View(transactionRecord);
        }

        // POST: TransactionRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CartId,SellerId,ItemCosts,ShippingCost,Fees,ShippingRecordId")] TransactionRecord transactionRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactionRecord).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CartId = new SelectList(db.Carts, "Id", "Id", transactionRecord.CartId);
            ViewBag.SellerId = new SelectList(db.UserRecords, "Id", "HDBUserName", transactionRecord.SellerId);
            ViewBag.ShippingRecordId = new SelectList(db.ShippingRecords, "Id", "TrackingNumber", transactionRecord.ShippingRecordId);
            return View(transactionRecord);
        }

        // GET: TransactionRecords/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionRecord transactionRecord = await db.TransactionRecords.FindAsync(id);
            if (transactionRecord == null)
            {
                return HttpNotFound();
            }
            return View(transactionRecord);
        }

        // POST: TransactionRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TransactionRecord transactionRecord = await db.TransactionRecords.FindAsync(id);
            db.TransactionRecords.Remove(transactionRecord);
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
