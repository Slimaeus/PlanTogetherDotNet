using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs.Comments
{
    public class EditCommentDTO
    {
        [Required]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime UpdateDate { get; } = DateTime.UtcNow;
    }
}