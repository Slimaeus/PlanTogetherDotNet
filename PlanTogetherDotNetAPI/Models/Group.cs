using System.Collections.Generic;

namespace PlanTogetherDotNetAPI.Models
{
    public class Group : Entity
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string OwnerId { get; set; }
        public AppUser Owner { get; set; }

        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    }
}