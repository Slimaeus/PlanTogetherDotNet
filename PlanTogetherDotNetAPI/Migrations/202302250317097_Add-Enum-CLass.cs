namespace PlanTogetherDotNetAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnumCLass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "CompletedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Missions", "CompletedDate");
        }
    }
}
