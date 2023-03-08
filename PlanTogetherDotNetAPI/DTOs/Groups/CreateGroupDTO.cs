using System.ComponentModel.DataAnnotations;

namespace PlanTogetherDotNetAPI.DTOs.Group
{
    public class CreateGroupDTO
    {
        [Required]
        [RegularExpression("^[a-z0-9-]+$")]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}