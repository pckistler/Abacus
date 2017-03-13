namespace Abacus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cartfeeschange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "HobbyDBFees", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "HobbyDBFees");
        }
    }
}
