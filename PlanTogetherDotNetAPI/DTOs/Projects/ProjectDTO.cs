using Newtonsoft.Json;
using PlanTogetherDotNetAPI.DTOs.Common;
using System;
using System.Collections.Generic;

namespace PlanTogetherDotNetAPI.DTOs.Project
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonProperty("Members")]
        public ICollection<MemberDTO> ProjectUsers { get; set; } = new HashSet<MemberDTO>();
    }
}