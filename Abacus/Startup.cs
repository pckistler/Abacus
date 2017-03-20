using Abacus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Abacus.Startup))]
namespace Abacus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            CreateRolesandUsers();
        }
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();

        //    // Add Identity services to the services container.
        //    services.AddDefaultIdentity<context, ApplicationUser, IdentityRole>(Configuration,
        //        o => {
        //            o.Password.RequireDigit = false;
        //            o.Password.RequireLowercase = false;
        //            o.Password.RequireUppercase = false;
        //            o.Password.RequireNonLetterOrDigit = false;
        //            o.Password.RequiredLength = 1;
        //        });
        //}
        // In this method we will create default User roles and Admin user for login   
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {
                
                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "peter";
                user.Email = "peter@thekistlers.com";

                string userPWD = "HotWheels";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }

                user = new ApplicationUser();
                user.UserName = "andrew";
                user.Email = "andrew@hobbydb.com";
                userPWD = "h0bbyd6";

                chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Disabled"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Disabled";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "Bob";
                //user.Email = "andrew@hobbydb.com";
                var userPWD = "bob";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Disabled");

                }
            }

        }
    }
}
