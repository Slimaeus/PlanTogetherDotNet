namespace PlanTogetherDotNetAPI.Data
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        ProjectId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ProjectId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropTable("dbo.ProjectUsers");
        }
    }
}
