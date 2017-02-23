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
            var list = db.UserRecords.OrderBy(u=>u.HDBUserName).ToList();
            cart.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
            cart.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
            ViewBag.BuyerId = cart.Buyers;
            ViewBag.SellerId = cart.Sellers;

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



        [HttpPost]
        public ActionResult HobbyDBUser(HobbyDBUser hdbUser)
        {
            if (ModelState.IsValid)
            {
                //int value = 0;
                //int id = 0;
                //if (Int32.TryParse(Request.Params[nameof(ViewModel.HobbyDBUser.Id)], out value))
                //    id = value;
                
                UserRecord user = db.UserRecords.SingleOrDefault(u => u.HDBUserId == hdbUser.HobbyDBUserId || string.Compare(u.HDBUserName, hdbUser.HobbyDBUserName, true) == 0);
                if (user != null)
                {
                    ModelState.AddModelError("", "User with the specified UserName or Id already exists");

                    return PartialView("_HobbyDBUser", hdbUser);
                }

                if (hdbUser.Id == 0)
                {
                    bool saveChanges = false;
                    Email preferredEmail = db.Emails.SingleOrDefault(e => string.Compare(e.EmailAddress, hdbUser.Email, true) == 0);
                    if (preferredEmail == null)
                    {
                        preferredEmail = new Email() { EmailAddress = hdbUser.Email };
                        db.Emails.Add(preferredEmail);
                        saveChanges = true;
                    }
                    Email payPalEmail = db.Emails.SingleOrDefault(e => string.Compare(e.EmailAddress, hdbUser.PayPalEmail, true) == 0);
                    if (payPalEmail == null)
                    {
                        payPalEmail = new Email() { EmailAddress = hdbUser.PayPalEmail };
                        db.Emails.Add(payPalEmail);
                        saveChanges = true;
                    }
                    if (saveChanges)
                        db.SaveChanges();

                    UserRecord ur = new UserRecord()
                    {
                        HDBUserName = hdbUser.HobbyDBUserName,
                        HDBUserId = hdbUser.HobbyDBUserId,
                        UserType = (hdbUser.IsBuyer ? UserRecord.UserTypes.Buyer : 0) | (hdbUser.IsSeller ? UserRecord.UserTypes.Seller : UserRecord.UserTypes.None),
                        PreferredEmailId = preferredEmail.Id,
                        PayPalEmailId = payPalEmail.Id
                    };
                    db.UserRecords.Add(ur);
                    db.SaveChanges();
                }
            }

            var list = db.UserRecords.OrderBy(u=>u.HDBUserName).ToList();
            var isBuyer = Request.Params["DropDownTarget"] == "buyer_dropdown";
            if (isBuyer)
            {
                ViewBag.BuyerId = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
                ViewBag.DropDownName = "BuyerId";
            }
            else
            {
                ViewBag.SellerId = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
                ViewBag.DropDownName = "SellerId";
            }
            return PartialView("_UpdateDropdown");
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
