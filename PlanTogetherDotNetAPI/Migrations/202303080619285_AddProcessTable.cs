namespace PlanTogetherDotNetAPI.Data
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProcessTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Processes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        IsDone = c.Boolean(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Processes", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Processes", new[] { "ProjectId" });
            DropTable("dbo.Processes");
        }
    }
}
