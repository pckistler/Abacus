﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abacus.Models;
using Abacus.Utility;
using Abacus.ViewModel;
using Abacus.Models.Interfaces;

namespace Abacus.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Carts
        public ActionResult Index()
        {
            var logs = db.LogRecords.Where(l => l.RecordType == Utilities.RecordType.New && l.Guid == typeof(Cart).GUID).OrderByDescending(l=>l.DateTime).Take(15);
            var list = logs.Select(l => db.Carts.FirstOrDefault(c => c.Id == l.RecordId));
            var cartSearch = Session["CartSearch"] ?? new CartSearch<int>() { SearchType = nameof(Cart.SearchOptions.None) };
            CartSearch<int> iSearch = cartSearch as CartSearch<int>;
            CartSearch<string> sSearch = cartSearch as CartSearch<string>;
            CartSearch<DateTime> dtSearch = cartSearch as CartSearch<DateTime>;
            IEnumerable<Cart> carts = null;
            if (iSearch != null) carts = ProcessSearch(iSearch);
            else if (sSearch != null) carts = ProcessSearch(sSearch);
            else if (dtSearch != null) carts = ProcessSearch(dtSearch);
            CartIndexVM ciVM = new CartIndexVM();
            ciVM.Carts = carts;
            ciVM.SearchOptions = new SelectList(Cart.SearchOptionNames, "Key", "Value");
            return View(ciVM);
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
            ViewBag.Buyers = cart.Buyers;
            ViewBag.Sellers = cart.Sellers;

            Session.Add("Transactions", new List<TransactionVM>());

            return View(cart);
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CartNumber,SaleDate,NumberOfItems,BuyerId,BuyerEmailId,CartAmount,ItemsAmount,ShippingAmount")] CartVM cartVM)
        {
            cartVM.TransactionVMs = Session["Transactions"] as List<TransactionVM>;

            if (ModelState.IsValid)
            {                
                bool error = false;
                if (cartVM.CartAmount <= 0)
                {
                    ModelState.AddModelError(nameof(cartVM.CartAmount), "The overall value must be larger than 0");
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
                    ModelState.AddModelError(nameof(cartVM.ItemsAmount), "The value of items cannot be larger than the overall value for the cart");
                    error = true;
                }
                if (cartVM.CartAmount < cartVM.ShippingAmount)
                {
                    ModelState.AddModelError(nameof(cartVM.ShippingAmount), "The shipping amount cannot be larger than the overall value for the cart");
                    error = true;
                }
                if (cartVM.CartAmount < cartVM.PayPalAmount)
                {
                    ModelState.AddModelError(nameof(cartVM.PayPalAmount), "The PayPal fees cannot be larger than the overall value for the cart");
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
                cart.ComputeFees(cartVM.TransactionVMs.Count);

                db.Carts.Add(cart);
                await db.SaveChangesAsync();
                LogRecord cartLR = new LogRecord()
                {
                    DateTime = DateTime.Now,
                    RecordType = Utilities.RecordType.New,
                    Guid = typeof(Cart).GUID,
                    RecordId = cart.Id
                };
                db.LogRecords.Add(cartLR);
                await db.SaveChangesAsync();

                ShippingCompany shipCompany = db.ShippingCompanies.FirstOrDefault(s => s.Name == "Unknown");

                foreach (var tr in cartVM.TransactionVMs)
                {
                    await AddTransaction(tr, cart.Id, shipCompany);                    
                }

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

            CartVM cartVM = new CartVM(cart);
            cartVM.TransactionVMs = TransactionVM.Transactions(cart.Transactions);

            var list = db.UserRecords.OrderBy(u => u.HDBUserName).ToList();
            cartVM.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
            cartVM.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
            ViewBag.BuyerId = cartVM.Buyers;
            ViewBag.SellerId = cartVM.Sellers;

            Session.Remove("Transactions");
            Session.Add("Transactions", cartVM.TransactionVMs);

            return View(cartVM);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind] CartVM cartVM)
        {
            cartVM.TransactionVMs = Session["Transactions"] as List<TransactionVM>;

            if (ModelState.IsValid)
            {
                bool error = false;
                if (cartVM.CartAmount <= 0)
                {
                    ModelState.AddModelError(nameof(cartVM.CartAmount), "The overall value in the cart must be larger than 0");
                    error = true;
                }
                if (cartVM.TransactionVMs == null || cartVM.TransactionVMs.Count ==0 )
                {
                    ModelState.AddModelError("NewSellerButton", "There must be at least 1 seller defined for the cart");
                    error = true;
                }
                if (cartVM.CartAmount < cartVM.ItemsAmount)
                {
                    ModelState.AddModelError(nameof(cartVM.ItemsAmount), "The value of items cannot be larger than the overall value for the cart");
                    error = true;
                }
                if (cartVM.CartAmount < cartVM.ShippingAmount)
                {
                    ModelState.AddModelError(nameof(cartVM.ShippingAmount), "The shipping amount cannot be larger than the overall value for the cart");
                    error = true;
                }

                double itemsCost = cartVM.TransactionVMs.Sum(t => t.ItemsTotal);
                if (cartVM.ItemsAmount != itemsCost)
                {
                    ModelState.AddModelError(nameof(cartVM.ItemsAmount), string.Format("The {0}, ${1:0.00}, must equal the sum of the seller Items, ${2:0.00}", nameof(cartVM.ItemsAmount), cartVM.ItemsAmount, itemsCost));
                    error = true;
                }
                double shipCost = cartVM.TransactionVMs.Sum(t => t.ShippingTotal);
                if (cartVM.ShippingAmount != shipCost)
                {
                    ModelState.AddModelError(nameof(cartVM.ShippingAmount), string.Format("The {0}, ${1:0.00}, must equal the sum of the seller shipping costs, ${2:0.00}", nameof(cartVM.ShippingAmount), cartVM.ShippingAmount, shipCost));
                    error = true;
                }
                List<int> sellers = cartVM.TransactionVMs.Select(t => t.SellerId).ToList();
                if (sellers.Distinct().Count() != sellers.Count)
                {
                    ModelState.AddModelError("cart_transaction_list", "The list of sellers contains one or more duplicated sellers");
                    error = true;
                }
                if (error)
                {
                    var list = db.UserRecords.OrderBy(u => u.HDBUserName).ToList();
                    cartVM.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");
                    cartVM.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
                    return View(cartVM);
                }

                var existingTransactions = db.TransactionRecords.Where(r => r.CartId == cartVM.Id).ToList();
                Cart cart = cartVM.Cart;
                cart.ComputeFees(cartVM.TransactionVMs.Count);
                db.Entry(cart).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Delete any transactions that have been removed
                List<int> deletedItems = new List<int>();
                foreach (var item in existingTransactions)
                {
                    if (!cartVM.TransactionVMs.Any(t => t.TransactionRecordId == item.Id))
                    {
                        deletedItems.Add(item.Id);
                    }
                }
                foreach (int id in deletedItems)
                {
                    TransactionRecord delTr = await db.TransactionRecords.FindAsync(id);
                    db.TransactionRecords.Remove(delTr);
                }

                // Add any new transactions
                List<int> newItems = new List<int>();
                foreach (var item in cartVM.TransactionVMs)
                {
                    if (!existingTransactions.Any(t => t.Id == item.TransactionRecordId))
                        newItems.Add(item.TransactionRecordId);
                }
                foreach (int id in newItems)
                {
                    var tr = cartVM.TransactionVMs.Find(r => r.TransactionRecordId == id);
                    await AddTransaction(tr, cart.Id, null);
                    cartVM.TransactionVMs.Remove(tr);
                }
                foreach (var item in cartVM.TransactionVMs)
                {
                    TransactionRecord tr = existingTransactions.Find(r=>r.Id == item.TransactionRecordId);
                    if (tr != null)
                    {
                        tr.ItemCosts = item.ItemsTotal;
                        tr.ShippingCost = item.ShippingTotal;
                        tr.SellerId = item.SellerId;
                        tr.ShippingRecord.TrackingNumber = item.TrackingNumber;
                        tr.NumOfItems = item.NumItems;
                        tr.ComputeFees();
                        db.Entry(tr).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                    //TransactionRecord tr = cartVM.Transactions.ElementAtOrDefault(0);

                    //if (tr != null)
                    //{
                    //    db.Entry(tr).State = EntityState.Modified;
                    //    tr.CartId = cart.Id;
                    //    await db.SaveChangesAsync();
                    //}                
                    //await db.SaveChangesAsync();
                    return RedirectToAction("Index");
            }
            //ViewBag.BuyerId = new SelectList(db.UserRecords, "Id", "HDBUserName", cart.BuyerId);
            //ViewBag.BuyerEmailId = new SelectList(db.Emails, "Id", "EmailAddress", cart.BuyerEmailId);
            return View(cartVM);
        }

        private async Task AddTransaction(TransactionVM tr, int cartId, ShippingCompany shipCompany)
        {
            if (shipCompany == null)
                shipCompany = db.ShippingCompanies.FirstOrDefault(s => s.Name == "Unknown");

            ShippingRecord sr = new ShippingRecord()
            {
                ShippingCompanyId = shipCompany.Id, 
                TrackingNumber = tr.TrackingNumber
            };            
            db.ShippingRecords.Add(sr);
            await db.SaveChangesAsync();

            TransactionRecord rec = tr.TransactionRecord;
            rec.ComputeFees();
            rec.CartId = cartId;
            rec.ShippingRecordId = sr.Id;
            rec.ShippingRecord = sr;
            db.TransactionRecords.Add(rec);
            await db.SaveChangesAsync();

            LogRecord shippingLog = new LogRecord()
            {
                DateTime = DateTime.Now,
                RecordType = Utilities.RecordType.New,
                Guid = typeof(ShippingRecord).GUID,
                RecordId = sr.Id
            };
            db.LogRecords.Add(shippingLog);

            LogRecord transactionLog = new LogRecord()
            {
                DateTime = DateTime.Now,
                RecordType = Utilities.RecordType.New,
                Guid = typeof(TransactionRecord).GUID,
                RecordId = rec.Id
            };
            db.LogRecords.Add(transactionLog);
            await db.SaveChangesAsync();

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
                    return View(); // PartialView("_HobbyDBUser", hdbUser);

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
                    return View();// PartialView("_HobbyDBUser", hdbUser);
                }

                if (error)
                    return View();// PartialView("_HobbyDBUser", hdbUser);

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
                        PhoneNumber = hdbUser.PhoneNumber,
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
            if (hdbUser.IsBuyer)
            {
                ViewBag.DropDownType = true;
                ViewBag.tabIndex = 5;
                cart.Buyers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer), "Id", "HDBUserName");

                return PartialView("_UpdateDropdown", cart);
            }
            return new ContentResult();
        }

        
        public ActionResult HobbyDBUserDialog()
        {
            int value, id = 0;
            if (Int32.TryParse(Request.Params["Id"], out value))
                id = value;
            bool isBuyer = string.Compare(Request.Params["UserType"], "buyer", true) == 0;
            bool isSeller = string.Compare(Request.Params["UserType"], "seller", true) == 0;

            HobbyDBUserVM ur = new ViewModel.HobbyDBUserVM() { IsNewRecord = (id == 0), Id = id, IsBuyer = isBuyer, IsSeller = isSeller };            
            
            if (id > 0)
            {
                UserRecord user = db.UserRecords.SingleOrDefault(u => u.Id == id);
                ur = new ViewModel.HobbyDBUserVM(user);
            }
            ur.Dialog.Controller = Request.Params["dlgCtrl"];
            ur.Dialog.Method = Request.Params["dlgMethod"];
            ur.Dialog.SuccessMethod = Request.Params["dlgSuccess"];
            ur.Dialog.FailureMethod = Request.Params["dlgFailure"];
            ur.Dialog.Target = Request.Params["dlgTarget"];
            ur.Dialog.Title = Request.Params["dlgTitle"];
            ur.Dialog.PartialView = Request.Params["dlgPartialView"];

            ActionResult ar = PartialView("_HobbyDBUser", ur);
            return PartialView("_HobbyDBUser", ur);
        }

        public ActionResult TransactionDialogContents()
        {
            int id = 0;            

            TransactionVM tr = new TransactionVM();
            if (Int32.TryParse(Request.Params["Id"], out id))
            {
                TransactionRecord user = db.TransactionRecords.SingleOrDefault(u => u.Id == id);
                if (user != null)
                {
                    tr = new TransactionVM(user);

                }
                int nItems = int.Parse(Request["nItems"]);
                double iAmount = double.Parse(Request["iAmount"]);
                double sAmount = double.Parse(Request["sAmount"]);
                var transactions = Session["Transactions"] as List<TransactionVM>;
                if (transactions != null)
                {
                    nItems -= transactions.Sum(t => t.NumItems);
                    iAmount -= transactions.Sum(t => t.ItemsTotal);
                    sAmount -= transactions.Sum(t => t.ShippingTotal);
                }
                tr.NumItems = nItems;
                tr.ItemsTotal = iAmount;
                tr.ShippingTotal = sAmount;
            }
            else
            {
                Guid g = Guid.Empty;
                if (Guid.TryParse(Request.Params["Id"], out g))
                {
                    var transactions = Session["Transactions"] as List<TransactionVM>;
                    if (transactions != null)
                    {
                        var item = transactions.Find(i => i.Id == g);
                        if (item != null)
                        {
                            tr = item;
                            tr.UpdateTargetId = string.Format("XAct{0}", tr.Id);
                        }
                    }                    
                }
            }

            var list = db.UserRecords.OrderBy(u => u.HDBUserName).ToList();
            tr.Sellers = new SelectList(list.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller), "Id", "HDBUserName");
            var pv = PartialView("_TransactionDlg", tr);
            return pv;
        }

        [HttpPost]
        public ActionResult CartTransaction(Abacus.ViewModel.TransactionVM xact)
        {
            var list = Session["Transactions"] as List<TransactionVM>;
            if (list != null)
            {
                var seller = db.UserRecords.Find(xact.SellerId);
                xact.SellerName = seller.HDBUserName;

                var item = list.Find(i => i.Id == xact.Id);
                if (item != null)
                {
                    item.SellerId = xact.SellerId;
                    item.NumItems = xact.NumItems;
                    item.SellerName = xact.SellerName;
                    item.ItemsTotal = xact.ItemsTotal;
                    item.ShippingTotal = xact.ShippingTotal;
                    item.TrackingNumber = xact.TrackingNumber;
                    return PartialView("_TransactionListItem", item);
                }
                else
                {
                    list.Add(xact);
                }
            }
            return PartialView("_TransactionList", list);
        }

        [HttpGet]
        public ActionResult DeleteTransaction()
        {
            var list = Session["Transactions"] as List<TransactionVM>;
            if (list != null && list.Count > 0)
            {
                Guid g = Guid.Empty;
                if (Guid.TryParse(Request.Params["Id"], out g))
                {
                    var item = list.Find(i => i.Id == g);
                    if (item != null)
                    {
                        list.Remove(item);
                    }
                }                
            }
            return PartialView("_TransactionList", list);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string SearchOptions()
        {
            var searchType = Request.Params["type"];
            var loadSession = bool.Parse(Request.Params["LoadSession"]);
            string result = string.Empty;
            switch(searchType)
            {
                case nameof(Cart.SearchOptions.BuyerEmail):
                    {
                        int id = 0;
                        if (loadSession)
                        {
                            CartSearch<int> session = Session["CartSearch"] as CartSearch<int>;
                            id = session.Value;
                        }

                        var list = db.UserRecords.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer).Select(r => new { r.PreferredEmail.EmailAddress, r.PreferredEmailId }).OrderBy(o => o.EmailAddress);
                        result = "<select class=\"form-control\" data-val=\"true\" data-val-number=\"The field BuyerId must be a number.\" data-val-required=\"The BuyerId field is required.\" id=\"" + searchType + "\" name=\"" + searchType + "\">";
                        var duplicates = new List<string>();
                        string selected;
                        foreach (var item in list)
                        {
                            selected = id == item.PreferredEmailId ? "selected" : string.Empty;
                            if (!duplicates.Contains(item.EmailAddress))
                            {
                                result += string.Format("<option value = \"{0}\" {2}>{1}</ option >", item.PreferredEmailId, item.EmailAddress, selected);
                                duplicates.Add(item.EmailAddress);
                            }
                        }
                        result += "</select>";
                        return result;
                    }
                case nameof(Cart.SearchOptions.SellerName):
                case nameof(Cart.SearchOptions.BuyerName):
                    {
                        string value = string.Empty;
                        if (loadSession)
                        {
                            CartSearch<string> session = Session["CartSearch"] as CartSearch<string>;
                            value = string.Format("value='{0}'", session.Value);
                        }
                        return "<input class=\"form-control text-box single-line\" data-val=\"true\" id=\"" + searchType + "\" name=\"" + searchType + "\" type=\"text\"" + value +" >";
                    }
                case nameof(Cart.SearchOptions.BuyerUsername):
                    {
                        int id = 0;
                        if (loadSession)
                        {
                            CartSearch<int> session = Session["CartSearch"] as CartSearch<int>;
                            id = session.Value;
                        }

                        var list = db.UserRecords.Where(u => (u.UserType & UserRecord.UserTypes.Buyer) == UserRecord.UserTypes.Buyer).Select(r => new { r.HDBUserName, r.Id, }).OrderBy(o => o.HDBUserName);
                        result = "<select class=\"form-control\" data-val=\"true\"  id=\"" + searchType + "\" name=\"" + searchType + "\">";
                        string selected;
                        foreach (var item in list)
                        {
                            selected = id == item.Id ? "selected" : string.Empty;
                            result += string.Format("<option value = \"{0}\" {2}>{1}</ option >", item.Id, item.HDBUserName, selected);
                        }
                        result += "</select>";
                        return result;
                    }
                case nameof(Cart.SearchOptions.CartNumber):
                    {
                        string value = string.Empty;
                        if (loadSession)
                        {
                            CartSearch<string> session = Session["CartSearch"] as CartSearch<string>;
                            value = string.Format("value='{0}'", session.Value);
                        }
                        return "<input class=\"form-control text-box single-line valid\" data-val=\"true\" data-val-number=\"The field Cart must be a number.\" data-val-required=\"The Cart field is required.\" id=\"" + searchType + "\" name=\"" + searchType + "\" type=\"number\" " + value + " >";
                    }
                case nameof(Cart.SearchOptions.Date):
                    {
                        DateTime d1 = DateTime.Now;
                        DateTime d2 = DateTime.Now;
                        if (loadSession)
                        {
                            CartSearch<DateTime> session = Session["CartSearch"] as CartSearch<DateTime>;
                            d1 = session.Value;
                            d2 = session.Value2;
                        }
                        result = "<label>Begin</label>&nbsp;<input class=\"text-box single-line valid\" data-val=\"true\" data-val-date=\"The field must be a date.\" data-val-required=\"The field is required.\" id=\"" + searchType + "\" name=\"" + searchType + "\" type=\"date\" value=\"" + d1.ToShortDateString() + "\" aria-required=\"true\" aria-describedby=\"Date-error\" aria-invalid=\"false\">";
                        result += "&nbsp;&nbsp;&nbsp;<label>End</label>&nbsp;<input class=\"text-box single-line valid\" data-val=\"true\" data-val-date=\"The field must be a date.\" data-val-required=\"The field is required.\" id=\"Date2\" name=\"Date2\" type=\"date\" value=\"" + d2.ToShortDateString() + "\" aria-required=\"true\" aria-describedby=\"Date-error\" aria-invalid=\"false\">";
                        return result;
                    }
                case nameof(Cart.SearchOptions.SellerUsername):
                    {
                        int id = 0;
                        if (loadSession)
                        {
                            CartSearch<int> session = Session["CartSearch"] as CartSearch<int>;
                            id = session.Value;
                        }

                        var list = db.UserRecords.Where(u => (u.UserType & UserRecord.UserTypes.Seller) == UserRecord.UserTypes.Seller).Select(r => new { r.HDBUserName, r.Id, }).OrderBy(o => o.HDBUserName);
                        result = "<select class=\"form-control\" data-val=\"true\"  id=\"" + searchType + "\" name=\"" + searchType + "\">";
                        foreach (var item in list)
                        {
                            result += string.Format("<option value = \"{0}\" {2}>{1}</ option >", item.Id, item.HDBUserName, (id==item.Id?"selected":""));
                        }
                        result += "</select>";
                        return result;
                    }
                case nameof(Cart.SearchOptions.None):
                    return "";
            }

            return "<label>"+searchType+"</label>";
        }

        public class CartSearch<ValueType> : ISearchType
        {
            public string SearchType { get; set; }
            public ValueType Value { get; set; }
            public ValueType Value2 { get; set; }
        }

        public ActionResult ProcessSearch()
        {
            var searchType = Request.Params["type"];
            string result = string.Empty;
            Session["CartSearch"] = null;
            switch (searchType)
            {
                case nameof(Cart.SearchOptions.None):
                    {                        
                        var cartSearch = new CartSearch<int> { SearchType = searchType };                        
                        var list = ProcessSearch(cartSearch);
                        return PartialView("_CartList", list);
                    }
                case nameof(Cart.SearchOptions.BuyerEmail):
                    {
                        int data = Int32.Parse(Request.Params["value"]);
                        var cartSearch = new CartSearch<int> { SearchType = searchType, Value = data };
                        Session["CartSearch"] = cartSearch;
                        var carts = ProcessSearch(cartSearch);
                        if (carts != null && carts.Count() > 0)
                            return PartialView("_CartList", carts.ToList());
                        else
                            return new ContentResult() { Content = "No carts found" };                        
                    }
                case nameof(Cart.SearchOptions.BuyerName):
                    {
                        string data = Request.Params["value"];
                        var cartSearch = new CartSearch<string> { SearchType = searchType, Value = data };
                        Session["CartSearch"] = cartSearch;
                        var carts = ProcessSearch(cartSearch);
                        if (carts != null && carts.Count() > 0)
                            return PartialView("_CartList", carts.ToList());
                        else
                            return new ContentResult() { Content = "No carts found" };
                    }
                case nameof(Cart.SearchOptions.BuyerUsername):
                    {
                        int data = Int32.Parse(Request.Params["value"]);
                        var cartSearch = new CartSearch<int> { SearchType = searchType, Value = data };
                        Session["CartSearch"] = cartSearch;
                        var carts = ProcessSearch(cartSearch);
                        if (carts != null && carts.Count() > 0)
                            return PartialView("_CartList", carts.ToList());
                        else
                            return new ContentResult() { Content = "No carts found" };
                    }
                case nameof(Cart.SearchOptions.CartNumber):
                    {
                        string data = Request.Params["value"];
                        var cartSearch = new CartSearch<string> { SearchType = searchType, Value = data };
                        Session["CartSearch"] = cartSearch;
                        var carts = ProcessSearch(cartSearch);
                        if (carts != null && carts.Count() > 0)
                            return PartialView("_CartList", carts.ToList());
                        else
                            return new ContentResult() { Content = "No carts found" };
                    }
                case nameof(Cart.SearchOptions.Date):
                    {
                        var data = DateTime.Parse(Request.Params["value"]);
                        var data2 = DateTime.Parse(Request.Params["value2"]);
                        DateTime d1 = DateTime.Compare(data, data2) > 0 ? data2 : data;
                        DateTime d2 = DateTime.Compare(data, data2) > 0 ? data : data2;
                        var cartSearch = new CartSearch<DateTime> { SearchType = searchType, Value = d1, Value2 = d2 };
                        Session["CartSearch"] = cartSearch;
                        var carts = ProcessSearch(cartSearch);
                        if (carts != null && carts.Count() > 0)
                            return PartialView("_CartList", carts.ToList());
                        else
                            return new ContentResult() { Content = "No carts found" };
                    }
                case nameof(Cart.SearchOptions.SellerName):
                    {
                        string data = Request.Params["value"];
                        var cartSearch = new CartSearch<string> { SearchType = searchType, Value = data };
                        Session["CartSearch"] = cartSearch;
                        data = data.ToUpper();
                        var carts = ProcessSearch(cartSearch);
                        if (carts != null && carts.Count() > 0)
                            return PartialView("_CartList", carts.ToList());
                        else
                            return new ContentResult() { Content = "No carts found" };
                    }
                case nameof(Cart.SearchOptions.SellerUsername):
                    {
                        int data = Int32.Parse(Request.Params["value"]);
                        var cartSearch = new CartSearch<int> { SearchType = searchType, Value = data };
                        Session["CartSearch"] = cartSearch;
                        var carts = ProcessSearch(cartSearch);
                        if (carts != null && carts.Count() > 0)
                            return PartialView("_CartList", carts.ToList());
                        else
                            return new ContentResult() { Content = "No carts found" };
                    }
            }

            return null;
        }


        private IEnumerable<Cart> ProcessSearch(ISearchType cartSearch)
        {
            switch (cartSearch.SearchType)
            {
                case nameof(Cart.SearchOptions.None):
                    {
                        var logs = db.LogRecords.Where(l => l.RecordType == Utilities.RecordType.New && l.Guid == typeof(Cart).GUID).OrderByDescending(l => l.DateTime).Take(15);
                        var list = logs.Select(l => db.Carts.FirstOrDefault(c => c.Id == l.RecordId));
                        return list.ToList();
                    }
                case nameof(Cart.SearchOptions.BuyerEmail):
                    {
                        int value = (cartSearch as CartSearch<int>).Value;
                        var carts = db.Carts.Where(c => c.BuyerEmailId == value).OrderByDescending(r => r.Id);
                        return carts.ToList();
                    }
                case nameof(Cart.SearchOptions.BuyerUsername):
                    {
                        int value = (cartSearch as CartSearch<int>).Value;
                        var carts = db.Carts.Where(c => c.BuyerId == value).OrderByDescending(r => r.Id);
                        return carts.ToList();
                    }
                case nameof(Cart.SearchOptions.SellerUsername):
                    {
                        int value = (cartSearch as CartSearch<int>).Value;
                        var carts = db.Carts.Where(c => c.Transactions.Any(t => t.SellerId == value)).OrderByDescending(r => r.Id);
                        return carts.ToList();
                    }
                case nameof(Cart.SearchOptions.Date):
                    {
                        DateTime d1 = (cartSearch as CartSearch<DateTime>).Value;
                        DateTime d2 = (cartSearch as CartSearch<DateTime>).Value;
                        var carts = db.Carts.Where(c => d1 <= c.SaleDate && c.SaleDate <= d2).OrderByDescending(r => r.Id);
                        return carts.ToList();
                    }
                case nameof(Cart.SearchOptions.BuyerName):
                    {
                        string name = (cartSearch as CartSearch<string>).Value.ToUpper();
                        var carts = db.Carts.Where(c => c.Buyer.FirstName.ToUpper().Contains(name) || c.Buyer.LastName.ToUpper().Contains(name)).OrderByDescending(r => r.Id);
                        return carts.ToList();
                    }
                case nameof(Cart.SearchOptions.CartNumber):
                    {
                        string value = (cartSearch as CartSearch<string>).Value.ToUpper();
                        var carts = db.Carts.Where(c => c.CartNumber.ToString().Contains(value)).OrderByDescending(r => r.Id);
                        return carts.ToList();
                    }
                case nameof(Cart.SearchOptions.SellerName):
                    {
                        string name = (cartSearch as CartSearch<string>).Value.ToUpper();
                        var carts = db.Carts.Where(c => c.Transactions.Any(t => t.Seller.FirstName.ToUpper().Contains(name) || t.Seller.LastName.ToUpper().Contains(name))).OrderByDescending(r => r.Id);
                        return carts.ToList();
                    }
            }

            return null;
        }

    }
}
