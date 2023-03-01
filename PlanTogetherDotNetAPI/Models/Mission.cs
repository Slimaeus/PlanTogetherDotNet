using PlanTogetherDotNetAPI.Enums;
using System;
using System.Collections.Generic;

namespace PlanTogetherDotNetAPI.Models
{
    public class Mission : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public MissionPriorities Priority { get; set; } = MissionPriorities.Low;
        public MissionStates State { get; set; } = MissionStates.New;
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public DateTime CompletedDate { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public ICollection<MissionUser> MissionUsers { get; set; } = new HashSet<MissionUser>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}