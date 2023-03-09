using Newtonsoft.Json;
using PlanTogetherDotNetAPI.DTOs.Comments;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.Enums;
using System;
using System.Collections.Generic;

namespace PlanTogetherDotNetAPI.DTOs
{
    public class MissionDTO : EntityDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public MissionPriorities Priority { get; set; }
        public MissionStates State { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public DateTime CompletedDate { get; set; } 
        public DateTime CreateDate { get; set; }
    }
}