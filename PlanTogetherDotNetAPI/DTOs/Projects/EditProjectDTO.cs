using PlanTogetherDotNetAPI.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace PlanTogetherDotNetAPI.DTOs.Project
{
    public class EditProjectDTO : EditDTO
    {
        [RegularExpression("^[a-z0-9-]+$")]
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}