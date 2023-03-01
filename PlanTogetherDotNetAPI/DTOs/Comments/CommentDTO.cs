using PlanTogetherDotNetAPI.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs.Comments
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public MemberDTO Owner { get; set; }
    }
}