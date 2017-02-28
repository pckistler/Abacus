namespace Abacus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogRecords : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogRecords", "RecordId", c => c.Int(nullable: false));
            AddColumn("dbo.LogRecords", "Guid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LogRecords", "Guid");
            DropColumn("dbo.LogRecords", "RecordId");
        }
    }
}
