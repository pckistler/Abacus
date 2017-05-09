namespace Abacus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRecords", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserRecords", "Notes");
        }
    }
}
