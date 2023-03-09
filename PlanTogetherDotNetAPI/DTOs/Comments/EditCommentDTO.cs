using Newtonsoft.Json;
using PlanTogetherDotNetAPI.DTOs.Common;
using System;

namespace PlanTogetherDotNetAPI.DTOs.Comments
{
    public class EditCommentDTO : EditDTO
    {
        public string Content { get; set; }
        [JsonIgnore]
        public DateTime UpdateDate { get; } = DateTime.UtcNow;
    }
}