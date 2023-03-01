using Microsoft.AspNet.Identity.EntityFramework;
using PlanTogetherDotNetAPI.Models;
using System.Configuration;
using System.Data.Entity;

namespace PlanTogetherDotNetAPI.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext() : base(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, DataInitializer>());
            //Database.SetInitializer(new DropCreateDatabaseAlways<DataContext>());
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;
        }

        public DbSet<Mission> Missions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<MissionUser> MissionUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MissionUser>()
                .HasKey(mu => new { mu.UserId, mu.MissionId });

        }
    }
}