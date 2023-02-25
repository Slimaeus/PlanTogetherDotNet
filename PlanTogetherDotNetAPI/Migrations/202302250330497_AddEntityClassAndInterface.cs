namespace PlanTogetherDotNetAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityClassAndInterface : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Projects", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "CreateDate");
            DropColumn("dbo.Missions", "CreateDate");
        }
    }
}
