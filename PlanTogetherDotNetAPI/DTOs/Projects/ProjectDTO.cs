using PlanTogetherDotNetAPI.DTOs.Common;
using System;

namespace PlanTogetherDotNetAPI.DTOs.Project
{
    public class ProjectDTO : EntityDTO
    {
        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}