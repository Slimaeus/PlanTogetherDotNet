using PlanTogetherDotNetAPI.DTOs.Common;

namespace PlanTogetherDotNetAPI.DTOs.Group
{
    public class GroupDTO : EntityDTO
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public MemberDTO Owner { get; set; }
    }
}