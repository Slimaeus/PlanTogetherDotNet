using System;

namespace PlanTogetherDotNetAPI.Models
{
    public class Process : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; } = false;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
    }
}