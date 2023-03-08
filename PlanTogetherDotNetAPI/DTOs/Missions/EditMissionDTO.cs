using PlanTogetherDotNetAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs
{
    public class EditMissionDTO
    {
        [Required]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [EnumDataType(typeof(MissionPriorities))]
        public MissionPriorities Priority { get; set; }
        [EnumDataType(typeof(MissionStates))]
        public MissionStates State { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}