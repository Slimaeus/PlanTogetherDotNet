namespace PlanTogetherDotNetAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMissionModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Missions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Priority = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Missions");
        }
    }
}
