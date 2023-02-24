using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}