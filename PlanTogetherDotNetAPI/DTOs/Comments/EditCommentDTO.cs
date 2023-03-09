using PlanTogetherDotNetAPI.DTOs.Common;
using System;

namespace PlanTogetherDotNetAPI.DTOs.Comments
{
    public class EditCommentDTO : EditDTO
    {
        public string Content { get; set; }
        public DateTime UpdateDate { get; } = DateTime.UtcNow;
    }
}