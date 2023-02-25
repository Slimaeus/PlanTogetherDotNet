using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs.Group
{
    public class CreateGroupDTO
    {
        [Required]
        [RegularExpression("^[a-z-]+$")]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}