using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace PlanTogetherDotNetAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public ICollection<MissionUser> MissionUsers { get; set; } = new HashSet<MissionUser>();
        public ICollection<Group> Groups { get; set; } = new HashSet<Group>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}