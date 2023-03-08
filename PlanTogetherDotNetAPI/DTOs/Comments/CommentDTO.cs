using PlanTogetherDotNetAPI.DTOs.Common;
using System;

namespace PlanTogetherDotNetAPI.DTOs.Comments
{
    public class CommentDTO : EntityDTO
    {
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public MemberDTO Owner { get; set; }
    }
}