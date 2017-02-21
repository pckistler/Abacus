namespace Abacus.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<Abacus.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Abacus.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.ShippingCompanies.AddOrUpdate(s => s.Name,
                new ShippingCompany { Name = "Unknown" });

            context.Countries.AddOrUpdate(c => c.Name,
                new Country { Name = "United States of America", Abbreviation = "USA" });

            int countryId = context.Countries.Local.First(c => c.Abbreviation == "USA").Id;

            context.States.AddOrUpdate(s => s.Name,
                new Models.CountryState { Name = "Alabama", Abbreviation = "AL", CountryId = countryId },
                new Models.CountryState { Name = "Alaska", Abbreviation = "AK", CountryId = countryId },
                new Models.CountryState { Name = "Arizona", Abbreviation = "AZ", CountryId = countryId },
                new Models.CountryState { Name = "Arkansas", Abbreviation = "AR", CountryId = countryId },
                new Models.CountryState { Name = "California", Abbreviation = "CA", CountryId = countryId },
                new Models.CountryState { Name = "Colorado", Abbreviation = "CO", CountryId = countryId },
                new Models.CountryState { Name = "Connecticut", Abbreviation = "CT", CountryId = countryId },
                new Models.CountryState { Name = "Delaware", Abbreviation = "DE", CountryId = countryId },
                new Models.CountryState { Name = "Florida", Abbreviation = "FL", CountryId = countryId },
                new Models.CountryState { Name = "Georgia", Abbreviation = "GA", CountryId = countryId },
                new Models.CountryState { Name = "Hawaii", Abbreviation = "HI", CountryId = countryId },
                new Models.CountryState { Name = "Illinois", Abbreviation = "ID", CountryId = countryId },
                new Models.CountryState { Name = "Indiana", Abbreviation = "IN", CountryId = countryId },
                new Models.CountryState { Name = "Iowa", Abbreviation = "IA", CountryId = countryId },
                new Models.CountryState { Name = "Kansas", Abbreviation = "KS", CountryId = countryId },
                new Models.CountryState { Name = "Kentucky", Abbreviation = "KY", CountryId = countryId },
                new Models.CountryState { Name = "Louisiana", Abbreviation = "LA", CountryId = countryId },
                new Models.CountryState { Name = "Maine", Abbreviation = "ME", CountryId = countryId },
                new Models.CountryState { Name = "Maryland", Abbreviation = "MD", CountryId = countryId },
                new Models.CountryState { Name = "Massachusetts", Abbreviation = "MA", CountryId = countryId },
                new Models.CountryState { Name = "Michigan", Abbreviation = "MI", CountryId = countryId },
                new Models.CountryState { Name = "Minnesota", Abbreviation = "MN", CountryId = countryId },
                new Models.CountryState { Name = "Mississippi", Abbreviation = "MS", CountryId = countryId },
                new Models.CountryState { Name = "Missouri", Abbreviation = "MO", CountryId = countryId },
                new Models.CountryState { Name = "Montana", Abbreviation = "MT", CountryId = countryId },
                new Models.CountryState { Name = "Nebraska", Abbreviation = "NE", CountryId = countryId },
                new Models.CountryState { Name = "Nevada", Abbreviation = "NV", CountryId = countryId },
                new Models.CountryState { Name = "New Hampshire", Abbreviation = "NH", CountryId = countryId },
                new Models.CountryState { Name = "New Jersey", Abbreviation = "NJ", CountryId = countryId },
                new Models.CountryState { Name = "New Mexico", Abbreviation = "NM", CountryId = countryId },
                new Models.CountryState { Name = "New York", Abbreviation = "NY", CountryId = countryId },
                new Models.CountryState { Name = "North Carolina", Abbreviation = "NC", CountryId = countryId },
                new Models.CountryState { Name = "North Dakota", Abbreviation = "ND", CountryId = countryId },
                new Models.CountryState { Name = "Ohio", Abbreviation = "OH", CountryId = countryId },
                new Models.CountryState { Name = "Oklahoma", Abbreviation = "OK", CountryId = countryId },
                new Models.CountryState { Name = "Oregon", Abbreviation = "OR", CountryId = countryId },
                new Models.CountryState { Name = "Pennsylvania", Abbreviation = "PA", CountryId = countryId },
                new Models.CountryState { Name = "Rhode Island", Abbreviation = "RI", CountryId = countryId },
                new Models.CountryState { Name = "South Carolina", Abbreviation = "SC", CountryId = countryId },
                new Models.CountryState { Name = "South Dakota", Abbreviation = "SD", CountryId = countryId },
                new Models.CountryState { Name = "Tennessee", Abbreviation = "TN", CountryId = countryId },
                new Models.CountryState { Name = "Texas", Abbreviation = "TX", CountryId = countryId },
                new Models.CountryState { Name = "Utah", Abbreviation = "UT", CountryId = countryId },
                new Models.CountryState { Name = "Vermont", Abbreviation = "VT", CountryId = countryId },
                new Models.CountryState { Name = "Virginia", Abbreviation = "VA", CountryId = countryId },
                new Models.CountryState { Name = "Washington", Abbreviation = "WA", CountryId = countryId },
                new Models.CountryState { Name = "West Virginia", Abbreviation = "WV", CountryId = countryId },
                new Models.CountryState { Name = "Wisconsin", Abbreviation = "WI", CountryId = countryId },
                new Models.CountryState { Name = "Wyoming", Abbreviation = "WY", CountryId = countryId });

            context.Emails.AddOrUpdate(e => e.EmailAddress,
                new Email { EmailAddress = "bogus@bogus.com" },
                new Email { EmailAddress = "test@bogus.com" },
                new Email { EmailAddress = "noone@bogus.com" }
                );

            //context.Addresses.AddOrUpdate(a => a.Street1,
            //    new Address { Street1 = "Any street", CountryId = countryId, StateId = 1, City= "AnyTown", PostalCode="00000" });
            //SeedUsers();
        }

        bool AddUserAndRole(Models.ApplicationDbContext context)
        {
            IdentityResult ir;
            var rm = new RoleManager<IdentityRole>
                (new RoleStore<IdentityRole>(context));
            ir = rm.Create(new IdentityRole("canEdit"));
            ir = rm.Create(new IdentityRole("canDelete"));
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            var user = new ApplicationUser()
            {
                UserName = "peter@thekistlers.com",
            };
            ir = um.Create(user, "P_assw0rd1");
            if (ir.Succeeded == false)
                return ir.Succeeded;
            ir = um.AddToRole(user.Id, "canEdit");
            ir = um.AddToRole(user.Id, "canDelete");
            return ir.Succeeded;
        }
        //private void SeedUsers()
        //{
        //    if (!WebSecurity.Initialized)
        //    {
        //        WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfiles", "UserId", "UserName", autoCreateTables: true);
        //    }

        //    var roles = System.Web.Security.Roles.Provider;
        //    var membership = (SimpleMembershipProvider)Membership.Provider;

        //    if (!roles.RoleExists("Admin"))
        //        roles.CreateRole("Admin");
        //    if (membership.GetUser("Peter", false) == null)
        //    {
        //        membership.CreateUserAndAccount("Peter", "1eskimo");
        //    }
        //    if (membership.GetUser("Goofus", false) == null)
        //    {
        //        membership.CreateUserAndAccount("Goofus", "kiwi");
        //    }
        //    if (!roles.GetRolesForUser("Peter").Contains("Admin"))
        //    {
        //        roles.AddUsersToRoles(new[] { "Peter" }, new[] { "Admin" });
        //    }
        //}
    }
}
