namespace PlanTogetherDotNetAPI.Data
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMissionIdToComment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Mission_Id", "dbo.Missions");
            DropIndex("dbo.Comments", new[] { "Mission_Id" });
            RenameColumn(table: "dbo.Comments", name: "Mission_Id", newName: "MissionId");
            AlterColumn("dbo.Comments", "MissionId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Comments", "MissionId");
            AddForeignKey("dbo.Comments", "MissionId", "dbo.Missions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "MissionId", "dbo.Missions");
            DropIndex("dbo.Comments", new[] { "MissionId" });
            AlterColumn("dbo.Comments", "MissionId", c => c.Guid());
            RenameColumn(table: "dbo.Comments", name: "MissionId", newName: "Mission_Id");
            CreateIndex("dbo.Comments", "Mission_Id");
            AddForeignKey("dbo.Comments", "Mission_Id", "dbo.Missions", "Id");
        }
    }
}
