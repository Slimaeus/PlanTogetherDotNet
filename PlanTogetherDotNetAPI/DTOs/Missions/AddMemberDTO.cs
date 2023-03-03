using System;

namespace PlanTogetherDotNetAPI.DTOs.Missions
{
    public class AddMemberDTO
    {
        public Guid MissionId { get; set; }
        public string UserName { get; set; }
    }
}