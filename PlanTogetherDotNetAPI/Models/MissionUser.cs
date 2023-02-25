using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.Models
{
    public class MissionUser
    {
        public Guid MissionId { get; set; }
        public Mission Mission { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}