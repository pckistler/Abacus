namespace Abacus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NumberOfItemsforTransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionRecords", "NumOfItems", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionRecords", "NumOfItems");
        }
    }
}
