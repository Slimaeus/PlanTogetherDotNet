using PlanTogetherDotNetAPI.DTOs.Comments;
using PlanTogetherDotNetAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs
{
    public class MissionDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public MissionPriorities Priority { get; set; }
        public MissionStates State { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public DateTime CompletedDate { get; set; } 
        public DateTime CreateDate { get; set; }
        public ICollection<CommentDTO> Comments { get; set; } = new HashSet<CommentDTO>();
    }
}