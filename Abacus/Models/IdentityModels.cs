using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Abacus.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("name=PostGRES", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<UserRecord> UserRecords { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryState> States { get; set; }
        //public DbSet<Address> Addresses { get; set; }
        public DbSet<ShippingCompany> ShippingCompanies { get; set; }
        public DbSet<ShippingRecord> ShippingRecords { get; set; }

        public DbSet<TransactionRecord> TransactionRecords { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}