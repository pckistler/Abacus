namespace Abacus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CartNumber = c.Int(nullable: false),
                        SaleDate = c.DateTime(nullable: false),
                        NumberOfItems = c.Int(nullable: false),
                        NumberOfSellers = c.Int(nullable: false),
                        BuyerId = c.Int(nullable: false),
                        BuyerEmailId = c.Int(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        ItemCost = c.Double(nullable: false),
                        ShippingCost = c.Double(nullable: false),
                        PayPalFees = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRecords", t => t.BuyerId, cascadeDelete: true)
                .ForeignKey("dbo.Emails", t => t.BuyerEmailId, cascadeDelete: true)
                .Index(t => t.BuyerId)
                .Index(t => t.BuyerEmailId);
            
            CreateTable(
                "dbo.UserRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HDBUserName = c.String(maxLength: 64),
                        HDBUserId = c.Int(nullable: false),
                        FirstName = c.String(maxLength: 64),
                        LastName = c.String(maxLength: 64),
                        PayPalEmailId = c.Int(nullable: false),
                        PreferredEmailId = c.Int(nullable: false),
                        UserType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Emails", t => t.PayPalEmailId, cascadeDelete: true)
                .ForeignKey("dbo.Emails", t => t.PreferredEmailId, cascadeDelete: true)
                .Index(t => t.PayPalEmailId)
                .Index(t => t.PreferredEmailId);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransactionRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CartId = c.Int(nullable: false),
                        SellerId = c.Int(nullable: false),
                        ItemCosts = c.Double(nullable: false),
                        ShippingCost = c.Double(nullable: false),
                        Fees = c.Double(nullable: false),
                        ShippingRecordId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.UserRecords", t => t.SellerId, cascadeDelete: true)
                .ForeignKey("dbo.ShippingRecords", t => t.ShippingRecordId, cascadeDelete: true)
                .Index(t => t.CartId)
                .Index(t => t.SellerId)
                .Index(t => t.ShippingRecordId);
            
            CreateTable(
                "dbo.ShippingRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShippingCompanyId = c.Int(nullable: false),
                        ShippedDate = c.DateTime(nullable: false),
                        TrackingNumber = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShippingCompanies", t => t.ShippingCompanyId, cascadeDelete: true)
                .Index(t => t.ShippingCompanyId);
            
            CreateTable(
                "dbo.ShippingCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256),
                        WebSite = c.String(maxLength: 256),
                        TrackingUrl = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChangeLogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LogRecordId = c.Long(nullable: false),
                        RecordType = c.String(maxLength: 64),
                        RecordID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LogRecords", t => t.LogRecordId, cascadeDelete: true)
                .Index(t => t.LogRecordId);
            
            CreateTable(
                "dbo.LogRecords",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        RecordType = c.Int(nullable: false),
                        Notes = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256),
                        Abbreviation = c.String(maxLength: 16),
                        PostalCodeFormat = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CountryStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256),
                        Abbreviation = c.String(maxLength: 16),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CountryStates", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.ChangeLogs", "LogRecordId", "dbo.LogRecords");
            DropForeignKey("dbo.TransactionRecords", "ShippingRecordId", "dbo.ShippingRecords");
            DropForeignKey("dbo.ShippingRecords", "ShippingCompanyId", "dbo.ShippingCompanies");
            DropForeignKey("dbo.TransactionRecords", "SellerId", "dbo.UserRecords");
            DropForeignKey("dbo.TransactionRecords", "CartId", "dbo.Carts");
            DropForeignKey("dbo.Carts", "BuyerEmailId", "dbo.Emails");
            DropForeignKey("dbo.Carts", "BuyerId", "dbo.UserRecords");
            DropForeignKey("dbo.UserRecords", "PreferredEmailId", "dbo.Emails");
            DropForeignKey("dbo.UserRecords", "PayPalEmailId", "dbo.Emails");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.CountryStates", new[] { "CountryId" });
            DropIndex("dbo.ChangeLogs", new[] { "LogRecordId" });
            DropIndex("dbo.ShippingRecords", new[] { "ShippingCompanyId" });
            DropIndex("dbo.TransactionRecords", new[] { "ShippingRecordId" });
            DropIndex("dbo.TransactionRecords", new[] { "SellerId" });
            DropIndex("dbo.TransactionRecords", new[] { "CartId" });
            DropIndex("dbo.UserRecords", new[] { "PreferredEmailId" });
            DropIndex("dbo.UserRecords", new[] { "PayPalEmailId" });
            DropIndex("dbo.Carts", new[] { "BuyerEmailId" });
            DropIndex("dbo.Carts", new[] { "BuyerId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Tests");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.CountryStates");
            DropTable("dbo.Countries");
            DropTable("dbo.LogRecords");
            DropTable("dbo.ChangeLogs");
            DropTable("dbo.ShippingCompanies");
            DropTable("dbo.ShippingRecords");
            DropTable("dbo.TransactionRecords");
            DropTable("dbo.Emails");
            DropTable("dbo.UserRecords");
            DropTable("dbo.Carts");
        }
    }
}
