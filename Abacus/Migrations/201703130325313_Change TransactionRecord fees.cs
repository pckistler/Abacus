namespace Abacus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTransactionRecordfees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionRecords", "PayPalFees", c => c.Double(nullable: false));
            AddColumn("dbo.TransactionRecords", "HobbyDBFees", c => c.Double(nullable: false));
            AddColumn("dbo.TransactionRecords", "PayOut", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionRecords", "PayOut");
            DropColumn("dbo.TransactionRecords", "HobbyDBFees");
            DropColumn("dbo.TransactionRecords", "PayPalFees");
        }
    }
}
