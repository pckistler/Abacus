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
    public class CartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Carts
        public async Task<ActionResult> Index()
        {
            var carts = db.Carts.Include(c => c.Buyer).Include(c => c.BuyerEmail);
            return View(await carts.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = await db.Carts.FindAsync(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        public ActionResult Create()
        {
            ViewModel.CartVM cart = new ViewModel.CartVM();
            var list = db.UserRecords.ToList();
            ViewBag.ExistingBuyerId = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
            ViewBag.SellerId = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");

            ViewBag.BuyerId = new SelectList(db.UserRecords, "Id", "HDBUserName");
            ViewBag.BuyerEmailId = new SelectList(db.Emails, "Id", "EmailAddress");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CartNumber,SaleDate,NumberOfItems,NumberOfSellers,BuyerId,BuyerEmailId,TotalValue,ItemCost,ShippingCost,PayPalFees")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BuyerId = new SelectList(db.UserRecords, "Id", "HDBUserName", cart.BuyerId);
            ViewBag.BuyerEmailId = new SelectList(db.Emails, "Id", "EmailAddress", cart.BuyerEmailId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = await db.Carts.FindAsync(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerId = new SelectList(db.UserRecords, "Id", "HDBUserName", cart.BuyerId);
            ViewBag.BuyerEmailId = new SelectList(db.Emails, "Id", "EmailAddress", cart.BuyerEmailId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CartNumber,SaleDate,NumberOfItems,NumberOfSellers,BuyerId,BuyerEmailId,TotalValue,ItemCost,ShippingCost,PayPalFees")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerId = new SelectList(db.UserRecords, "Id", "HDBUserName", cart.BuyerId);
            ViewBag.BuyerEmailId = new SelectList(db.Emails, "Id", "EmailAddress", cart.BuyerEmailId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = await db.Carts.FindAsync(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Cart cart = await db.Carts.FindAsync(id);
            db.Carts.Remove(cart);
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
