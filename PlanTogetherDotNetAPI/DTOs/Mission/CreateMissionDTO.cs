using PlanTogetherDotNetAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs
{
    public class CreateMissionDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [EnumDataType(typeof(MissionPriorities))]
        public MissionPriorities Priority { get; set; }
        [EnumDataType(typeof(MissionStates))]
        public MissionStates State { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow; 
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;

        public Guid ProjectId { get; set; }
    }
}