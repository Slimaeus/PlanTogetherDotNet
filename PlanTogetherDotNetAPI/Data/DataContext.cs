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
        }

        public DbSet<Mission> Missions { get; set; }
        public DbSet<Project> Projects { get; set; }
    }
}