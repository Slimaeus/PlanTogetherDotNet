using System.ComponentModel.DataAnnotations;

namespace PlanTogetherDotNetAPI.DTOs.Project
{
    public class CreateProjectDTO
    {
        [Required]
        [RegularExpression("^[a-z-]+$")]
        public string Name { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string GroupName { get; set; }
    }
}