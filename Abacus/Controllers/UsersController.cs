using Abacus.Models;
using Abacus.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Abacus.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private ApplicationSignInManager m_SignInManager;
        private ApplicationUserManager m_UserManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            userManager.PasswordValidator = new PasswordValidator()
            {
                RequireDigit = false,
                RequiredLength = 2,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false
            };
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return m_SignInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                m_SignInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (m_UserManager != null)
                    return m_UserManager;
                m_UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();                
                m_UserManager.PasswordValidator = new PasswordValidator()
                    {
                        RequireDigit = false,
                        RequiredLength = 2,
                        RequireLowercase = false,
                        RequireNonLetterOrDigit = false,
                        RequireUppercase = false
                    };                
                m_UserManager.UserValidator = new UserValidator<ApplicationUser>(m_UserManager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = false
                };
                return m_UserManager;
            }
            private set
            {
                m_UserManager = value;
            }
        }

        // GET: Users
        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                //	ApplicationDbContext context = new ApplicationDbContext();
                //	var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                //var s=	UserManager.GetRoles(user.GetUserId());
                ViewBag.displayMenu = "No";

                List<UserVM> users = new List<UserVM>();
                //ManageUsersListVM manageUsers = new ManageUsersListVM();
                if (isAdminUser())
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                    var roles = roleManager.Roles.ToList();

                    ViewBag.displayMenu = "Yes";
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var list = UserManager.Users.OrderBy(u=>u.UserName).ToList();
                    foreach (var item in list)
                    {
                        UserVM userVM = new UserVM() { Id = item.Id, UserName = item.UserName };
                        userVM.Role = roles.Find(r => r.Id == item.Roles.ElementAt(0).RoleId).Name;
                        users.Add(userVM);
                    }
                }
                return View(users);
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }


            return View();
        }

        //
        // GET: /Account/Register
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Name = new SelectList(db.Roles.ToList(), "Id", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName  };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Add default User to Role Admin   
                    var role = db.Roles.Find(model.UserRoles);
                    var resultRole = UserManager.AddToRole(user.Id, role.Name);

                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Users");
                }
                ViewBag.Name = new SelectList(db.Roles.ToList(), "Id", "Name");
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDialogContents()
        {
            string id = Guid.Empty.ToString();
            Guid guid;
            if (Guid.TryParse(Request.Params["Id"], out guid))
            {
                var user = UserManager.Users.SingleOrDefault(u => u.Id == guid.ToString());
                if (user != null)
                {
                    var role = string.Empty;
                    if (user.Roles.Count > 0)
                        role = db.Roles.Find(user.Roles.ElementAt(0).RoleId).Name;
                    UserVM userVM = new UserVM() { Id=user.Id, UserName = user.UserName, Role = role };
                    var pv = PartialView("_DeleteDlg", userVM);
                    return pv;
                }
            }          
            return new EmptyResult();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(UserVM userVM)
        {
            string id = Request.Params["Id"];
            var user = UserManager.Users.SingleOrDefault(u => u.Id == id);
            await UserManager.DeleteAsync(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}