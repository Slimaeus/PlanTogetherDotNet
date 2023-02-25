using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.Models
{
    public class Project : Entity
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}