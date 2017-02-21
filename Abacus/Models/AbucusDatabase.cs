using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Abucus.Models
{
    public class AbucusDatabase : DbContext
    {
        public AbucusDatabase() : base("name=PostGRES")
        {
            Database.SetInitializer<AbucusDatabase>(null);
        }
        public AbucusDatabase(string connectionString) : base (connectionString)   
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("public");

            //// user
            //var logRecord = modelBuilder.Entity<LogRecord>();
            //logRecord.ToTable("logrecord")
            //    .HasKey(t => t.ID)
            //    .Property(t => t.ID)
            //        .HasColumnName("id");

            // In case table columns need to be lower case
            //modelBuilder.Properties().Configure(p =>
            //{
            //    var name = p.ClrPropertyInfo.Name;                
            //    p.HasColumnName(name.ToLower());
            //});

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Test> Tests { get; set; }
        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<UserRecord> UserRecords { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<ShippingCompany> ShippingCompanies { get; set; }
        public DbSet<ShippingRecord> ShippingRecords { get; set; }
    }
}