using System;

namespace PlanTogetherDotNetAPI.DTOs.Comments
{
    public class CreateCommentDTO
    {
        public string Content { get; set; }

        public Guid MissionId { get; set; }
        public string UserName { get; set; }
    }
}