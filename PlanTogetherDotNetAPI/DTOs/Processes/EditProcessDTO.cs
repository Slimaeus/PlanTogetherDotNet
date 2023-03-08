using System;
using System.ComponentModel.DataAnnotations;

namespace PlanTogetherDotNetAPI.DTOs.Processes
{
    public class EditProcessDTO
    {
        [Required]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
    }
}