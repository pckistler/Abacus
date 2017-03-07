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
            var logs = db.LogRecords.Where(l => l.RecordType == Utilities.RecordType.New && l.Guid == typeof(Cart).GUID).OrderByDescending(l=>l.DateTime).Take(15);
            var xxx = logs.Select(l => db.Carts.FirstOrDefault(c => c.Id == l.RecordId));
            //var tmp = db.Carts.Where(c => logs.Any(l => l.RecordId == c.Id)).ToList() ;
            var carts = db.Carts.Include(c => c.Buyer).Include(c => c.BuyerEmail);
            return View(await xxx.ToListAsync());
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
            cart.SaleDate = DateTime.Now;
            cart.NumberOfItems = 1;
            cart.NumberOfSellers = 1;
            cart.BuyerId = 0;
            var list = db.UserRecords.OrderBy(u=>u.HDBUserName).ToList();
            cart.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
            cart.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
            ViewBag.BuyerId = cart.Buyers;
            ViewBag.SellerId = cart.Sellers;

            return View(cart);
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CartNumber,SaleDate,NumberOfItems,NumberOfSellers,BuyerId,BuyerEmailId,CartAmount,ItemsAmount,ShippingAmount,PayPalAmount,SellerId,SellerItemsTotal,SellerShippingTotal,TrackingNumber")] CartVM cartVM)
        {
            if (ModelState.IsValid)
            {
                bool error = false;
                if (cartVM.CartAmount <= 0)
                {
                    ModelState.AddModelError(nameof(cartVM.CartAmount), "The value in the cart must be larger than 0");
                    error = true;
                }
                if (cartVM.ItemsAmount <= 0)
                {
                    ModelState.AddModelError(nameof(cartVM.ItemsAmount), "The value of items in the cart must be larger than 0");
                    error = true;
                }
                if (cartVM.ShippingAmount < 0)
                {
                    ModelState.AddModelError(nameof(cartVM.ShippingAmount), "The shipping cost for items in the cart cannot be less than 0.");
                    error = true;
                }
                if (cartVM.PayPalAmount < 0)
                {
                    ModelState.AddModelError(nameof(cartVM.PayPalAmount), "The PayPal fees cannot be less than 0.");
                    error = true;
                }
                if (cartVM.CartAmount < cartVM.ItemsAmount)
                {
                    ModelState.AddModelError(nameof(cartVM.ItemsAmount), "The value of items cannot be larger than the total for the cart");
                    error = true;
                }
                if (cartVM.CartAmount < cartVM.ShippingAmount)
                {
                    ModelState.AddModelError(nameof(cartVM.ShippingAmount), "The shipping amount cannot be larger than the total for the cart");
                    error = true;
                }
                if (cartVM.CartAmount < cartVM.PayPalAmount)
                {
                    ModelState.AddModelError(nameof(cartVM.PayPalAmount), "The PayPal fees cannot be larger than the total for the cart");
                    error = true;
                }
                if (error)
                {
                    var list = db.UserRecords.OrderBy(u => u.HDBUserName).ToList();
                    cartVM.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
                    cartVM.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
                    return View(cartVM);
                }

                Cart cart = new Cart()
                {
                    CartNumber = cartVM.CartNumber,
                    SaleDate = cartVM.SaleDate,
                    NumberOfItems = cartVM.NumberOfItems,
                    NumberOfSellers = cartVM.NumberOfSellers,
                    BuyerId = cartVM.BuyerId,
                    TotalValue = cartVM.CartAmount,
                    ItemCost = cartVM.ItemsAmount,
                    ShippingCost = cartVM.ShippingAmount,
                    PayPalFees = cartVM.PayPalAmount,
                };
                cart.Buyer = db.UserRecords.SingleOrDefault(r => r.Id == cartVM.BuyerId);
                cart.BuyerEmailId = cart.Buyer.PreferredEmailId;
                LogRecord lrCart = new LogRecord()
                {
                    DateTime = DateTime.Now,
                    RecordType = Utilities.RecordType.New,
                    Guid = typeof(Cart).GUID
                };

                ShippingRecord sr = new ShippingRecord()
                {
                    ShippingCompanyId = db.ShippingCompanies.FirstOrDefault(s => s.Name == "Unknown").Id,
                    TrackingNumber = cartVM.TrackingNumber
                };
                LogRecord lrShippingRecord = new LogRecord()
                {
                    DateTime = DateTime.Now,
                    RecordType = Utilities.RecordType.New,
                    Guid = typeof(ShippingRecord).GUID
                };
                db.ShippingRecords.Add(sr);
                db.Carts.Add(cart);
                db.SaveChanges();

                TransactionRecord tr = new TransactionRecord()
                {
                    SellerId = cartVM.SellerId,
                    ItemCosts = cartVM.SellerItemsTotal,
                    ShippingCost = cartVM.SellerShippingTotal,
                    ShippingRecordId = sr.Id,
                    CartId = cart.Id
                };
                LogRecord lrTransaction = new LogRecord()
                {
                    DateTime = DateTime.Now,
                    RecordType = Utilities.RecordType.New,
                    Guid = typeof(TransactionRecord).GUID
                };
                db.TransactionRecords.Add(tr);
                db.SaveChanges();

                lrCart.RecordId = cart.Id;
                lrShippingRecord.RecordId = sr.Id;
                lrTransaction.RecordId = tr.Id;
                db.LogRecords.Add(lrCart);
                db.LogRecords.Add(lrShippingRecord);
                db.LogRecords.Add(lrTransaction);
                db.SaveChanges();

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(cartVM);
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
            //ViewBag.BuyerId = new SelectList(db.UserRecords, "Id", "HDBUserName", cart.BuyerId);
            //ViewBag.BuyerEmailId = new SelectList(db.Emails, "Id", "EmailAddress", cart.BuyerEmailId);

            CartVM cartVM = new CartVM(cart);
            var list = db.UserRecords.OrderBy(u => u.HDBUserName).ToList();
            cartVM.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
            cartVM.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
            ViewBag.BuyerId = cartVM.Buyers;
            ViewBag.SellerId = cartVM.Sellers;


            return View(cartVM);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind] CartVM cartVM)
        //public async Task<ActionResult> Edit([Bind(Include = "Id,CartNumber,SaleDate,NumberOfItems,NumberOfSellers,BuyerId,BuyerEmailId,CartAmount,ItemsAmount,ShippingAmount,PayPalAmount,SellerId,SellerItemsTotal,SellerShippingTotal,TrackingNumber")] CartVM cartVM)
        {
            if (ModelState.IsValid)
            {
                Cart cart = cartVM.Cart;
                TransactionRecord tr = cartVM.Transactions.ElementAtOrDefault(0);

                db.Entry(cart).State = EntityState.Modified;
                await db.SaveChangesAsync();

                if (tr != null)
                {
                    db.Entry(tr).State = EntityState.Modified;
                    tr.CartId = cart.Id;
                    await db.SaveChangesAsync();
                }                
                //await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //ViewBag.BuyerId = new SelectList(db.UserRecords, "Id", "HDBUserName", cart.BuyerId);
            //ViewBag.BuyerEmailId = new SelectList(db.Emails, "Id", "EmailAddress", cart.BuyerEmailId);
            return View(cartVM);
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

                UserRecord user = db.UserRecords.SingleOrDefault(u => u.HDBUserId == hdbUser.HobbyDBUserId || string.Compare(u.HDBUserName, hdbUser.HobbyDBUserName, true) == 0);
                if (user != null)
                {
                    string ctrl = user.HDBUserId == hdbUser.HobbyDBUserId ? nameof(hdbUser.HobbyDBUserId) : nameof(hdbUser.HobbyDBUserName);
                    if (user.HDBUserId == hdbUser.HobbyDBUserId)
                    {
                        ModelState.AddModelError(nameof(hdbUser.HobbyDBUserId), "User with the specified User Id already exists");
                    }
                    if (string.Compare(user.HDBUserName, hdbUser.HobbyDBUserName, true) ==0)
                    {
                        ModelState.AddModelError(nameof(hdbUser.HobbyDBUserName), "User with the specified User Id already exists");
                    }
                    return PartialView("_HobbyDBUser", hdbUser);
                }

                if (error)
                    return PartialView("_HobbyDBUser", hdbUser);

                if (hdbUser.Id == 0)
                {
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

                    UserRecord ur = new UserRecord()
                    {
                        HDBUserName = hdbUser.HobbyDBUserName,
                        HDBUserId = hdbUser.HobbyDBUserId,
                        LastName = hdbUser.LastName,
                        FirstName = hdbUser.FirstName,
                        UserType = (hdbUser.IsBuyer ? UserRecord.UserTypes.Buyer : 0) | (hdbUser.IsSeller ? UserRecord.UserTypes.Seller : UserRecord.UserTypes.None),
                        PreferredEmailId = preferredEmail.Id,
                        PayPalEmailId = payPalEmail.Id
                    };
                    db.UserRecords.Add(ur);
                    db.SaveChanges();
                    userRecordId = ur.Id;
                }
            }
            else
            {
                return PartialView(hdbUser);
            }

            CartVM cart = new CartVM();
            cart.BuyerId = userRecordId;
            cart.SellerId = userRecordId;
            var list = db.UserRecords.OrderBy(u=>u.HDBUserName).ToList();
            var isBuyer = Request.Params["DropDownTarget"] == "buyer_dropdown";
            ViewBag.DropDownType = isBuyer;
            ViewBag.tabIndex = isBuyer ? 5: 20;
            cart.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
            cart.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");

            return PartialView("_UpdateDropdown", cart);
        }

        public ActionResult ViewHobbyDBUser()
        {
            int value, id = 0;
            if (Int32.TryParse(Request.Params["Id"], out value))
                id = value;
            bool isBuyer = string.Compare(Request.Params["UserType"], "buyer") == 0;

            HobbyDBUser ur = new ViewModel.HobbyDBUser() { IsNewRecord = (id == 0), Id = id, IsBuyer = isBuyer, IsSeller = !isBuyer };
            if (id > 0)
            {
                UserRecord user = db.UserRecords.SingleOrDefault(u => u.Id == id);
                ur = new ViewModel.HobbyDBUser(user);
            }            

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
