using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace PlanTogetherDotNetAPI.DTOs
{
    public class EditMissionDTO : EditDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        [EnumDataType(typeof(MissionPriorities))]
        public MissionPriorities Priority { get; set; }
        [EnumDataType(typeof(MissionStates))]
        public MissionStates State { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        //public DateTime CompletedDate { get; set; }
    }
}