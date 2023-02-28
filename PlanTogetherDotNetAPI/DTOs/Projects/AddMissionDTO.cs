using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs.Project
{
    public class AddMissionDTO
    {
        public Guid ProjectId { get; set; }
        public Guid MissionId { get; set; }
    }
}