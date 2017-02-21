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
    public class ShippingCompaniesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShippingCompanies
        public async Task<ActionResult> Index()
        {
            return View(await db.ShippingCompanies.ToListAsync());
        }

        // GET: ShippingCompanies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingCompany shippingCompany = await db.ShippingCompanies.FindAsync(id);
            if (shippingCompany == null)
            {
                return HttpNotFound();
            }
            return View(shippingCompany);
        }

        // GET: ShippingCompanies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShippingCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,WebSite,TrackingUrl")] ShippingCompany shippingCompany)
        {
            if (ModelState.IsValid)
            {
                db.ShippingCompanies.Add(shippingCompany);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(shippingCompany);
        }

        // GET: ShippingCompanies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingCompany shippingCompany = await db.ShippingCompanies.FindAsync(id);
            if (shippingCompany == null)
            {
                return HttpNotFound();
            }
            return View(shippingCompany);
        }

        // POST: ShippingCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,WebSite,TrackingUrl")] ShippingCompany shippingCompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingCompany).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shippingCompany);
        }

        // GET: ShippingCompanies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingCompany shippingCompany = await db.ShippingCompanies.FindAsync(id);
            if (shippingCompany == null)
            {
                return HttpNotFound();
            }
            return View(shippingCompany);
        }

        // POST: ShippingCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShippingCompany shippingCompany = await db.ShippingCompanies.FindAsync(id);
            db.ShippingCompanies.Remove(shippingCompany);
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
