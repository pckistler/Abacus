namespace Abacus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payout : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payouts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SellerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRecords", t => t.SellerId, cascadeDelete: true)
                .Index(t => t.SellerId);
            
            AddColumn("dbo.UserRecords", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payouts", "SellerId", "dbo.UserRecords");
            DropIndex("dbo.Payouts", new[] { "SellerId" });
            DropColumn("dbo.UserRecords", "PhoneNumber");
            DropTable("dbo.Payouts");
        }
    }
}
