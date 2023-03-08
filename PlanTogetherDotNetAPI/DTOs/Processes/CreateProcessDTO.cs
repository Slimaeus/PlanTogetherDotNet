using System.ComponentModel.DataAnnotations;

namespace PlanTogetherDotNetAPI.DTOs.Processes
{
    public class CreateProcessDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsDone { get; set; }

        [Required]
        public string ProjectName { get; set; }
    }
}