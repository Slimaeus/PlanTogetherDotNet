namespace PlanTogetherDotNetAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMissionUsersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MissionUsers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        MissionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.MissionId })
                .ForeignKey("dbo.Missions", t => t.MissionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.MissionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MissionUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MissionUsers", "MissionId", "dbo.Missions");
            DropIndex("dbo.MissionUsers", new[] { "MissionId" });
            DropIndex("dbo.MissionUsers", new[] { "UserId" });
            DropTable("dbo.MissionUsers");
        }
    }
}
