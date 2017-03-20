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
    [Authorize]
    public class ShippingRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShippingRecords
        public async Task<ActionResult> Index()
        {
            var shippingRecords = db.ShippingRecords.Include(s => s.ShippingCompany);
            return View(await shippingRecords.ToListAsync());
        }

        // GET: ShippingRecords/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingRecord shippingRecord = await db.ShippingRecords.FindAsync(id);
            if (shippingRecord == null)
            {
                return HttpNotFound();
            }
            return View(shippingRecord);
        }

        // GET: ShippingRecords/Create
        public ActionResult Create()
        {
            ViewBag.ShippingCompanyId = new SelectList(db.ShippingCompanies, "Id", "Name");
            return View();
        }

        // POST: ShippingRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ShippingCompanyId,ShippedDate,TrackingNumber")] ShippingRecord shippingRecord)
        {
            if (ModelState.IsValid)
            {
                db.ShippingRecords.Add(shippingRecord);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ShippingCompanyId = new SelectList(db.ShippingCompanies, "Id", "Name", shippingRecord.ShippingCompanyId);
            return View(shippingRecord);
        }

        // GET: ShippingRecords/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingRecord shippingRecord = await db.ShippingRecords.FindAsync(id);
            if (shippingRecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.ShippingCompanyId = new SelectList(db.ShippingCompanies, "Id", "Name", shippingRecord.ShippingCompanyId);
            return View(shippingRecord);
        }

        // POST: ShippingRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ShippingCompanyId,ShippedDate,TrackingNumber")] ShippingRecord shippingRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingRecord).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ShippingCompanyId = new SelectList(db.ShippingCompanies, "Id", "Name", shippingRecord.ShippingCompanyId);
            return View(shippingRecord);
        }

        // GET: ShippingRecords/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingRecord shippingRecord = await db.ShippingRecords.FindAsync(id);
            if (shippingRecord == null)
            {
                return HttpNotFound();
            }
            return View(shippingRecord);
        }

        // POST: ShippingRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShippingRecord shippingRecord = await db.ShippingRecords.FindAsync(id);
            db.ShippingRecords.Remove(shippingRecord);
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
