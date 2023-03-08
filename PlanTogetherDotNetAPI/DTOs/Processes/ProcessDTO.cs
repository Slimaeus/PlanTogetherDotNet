using System;

namespace PlanTogetherDotNetAPI.DTOs.Processes
{
    public class ProcessDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
    }
}