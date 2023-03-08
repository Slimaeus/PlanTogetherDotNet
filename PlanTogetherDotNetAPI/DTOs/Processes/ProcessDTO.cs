using PlanTogetherDotNetAPI.DTOs.Common;
using System;

namespace PlanTogetherDotNetAPI.DTOs.Processes
{
    public class ProcessDTO : EntityDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
    }
}