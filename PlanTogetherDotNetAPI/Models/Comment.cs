using System;

namespace PlanTogetherDotNetAPI.Models
{
    public class Comment : Entity
    {
        public string Content { get; set; }
        public DateTime PostDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        public Mission Mission { get; set; }

        public AppUser Owner { get; set; }
    }
}