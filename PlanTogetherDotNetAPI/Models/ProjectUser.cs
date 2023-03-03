using System;

namespace PlanTogetherDotNetAPI.Models
{
    public class ProjectUser
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}