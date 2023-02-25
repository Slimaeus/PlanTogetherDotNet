using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs.Group
{
    public class GroupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public MemberDTO Owner { get; set; }

        public ICollection<ProjectDTO> Projects { get; set; } = new HashSet<ProjectDTO>();
    }
}