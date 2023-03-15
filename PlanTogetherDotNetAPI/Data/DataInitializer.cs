using PlanTogetherDotNetAPI.Enums;
using PlanTogetherDotNetAPI.Models;
using System.Collections.Generic;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PlanTogetherDotNetAPI.Data
{
    public class DataInitializer : DbMigrationsConfiguration<DataContext>
    {

        public DataInitializer()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DataContext context)
        {
            base.Seed(context);

            if (context.Roles.Any()) return;

            var roles = new IdentityRole[]
            {
                new IdentityRole { Name = "Admin" },
                new IdentityRole { Name = "ProjectManager" },
                new IdentityRole { Name = "GroupManager" }
            };

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var role in roles)
            {
                roleManager.Create(role);
            }

            if (context.Users.Any()) return;

            var users = new List<AppUser>
            {
                new AppUser
                {
                    DisplayName = "thai",
                    UserName = "thai",
                    Email = "thai@test.com",
                },
                new AppUser
                {
                    DisplayName = "mei",
                    UserName = "mei",
                    Email = "mei@test.com"
                },
                new AppUser
                {
                    DisplayName = "a",
                    UserName = "a",
                    Email = "a@test.com"
                },
                new AppUser
                {
                    DisplayName = "string",
                    UserName = "string",
                    Email = "string@test.com"
                }
            };

            var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));

            foreach (var user in users)
            {
                userManager.AddToRole(user.Id, roles[0].Name);
                userManager.Create(user, "P@ssw0rd");
            }

            if (context.Groups.Any()) return;
            var groups = new List<Group>
            {
                new Group
                {
                    Name = "semibox",
                    Title = "Semibox",
                    Description = "Half of a box",
                    Owner = users[1],
                },
                new Group
                {
                    Name = "owlvernyte",
                    Title = "Owlvernyte",
                    Description = "Overnight owl",
                    Owner = users[0],
                }
            };

            context.Groups.AddRange(groups);

            if (context.Projects.Any()) return;

            var projects = new List<Project>
            {
                new Project
                {
                    Name = "caro-online",
                    Title = "Caro Online",
                    Description = "Caro game",
                    ProjectUsers = new List<ProjectUser>
                    {
                        new ProjectUser
                        {
                            User = users[0]
                        }
                    },
                    Group = groups[1]
                },
                new Project
                {
                    Name = "plan-together",
                    Title = "Plan together",
                    Description = "Mission management platform",
                    ProjectUsers = new List<ProjectUser>
                    {
                        new ProjectUser
                        {
                            User = users[0]
                        },
                        new ProjectUser
                        {
                            User = users[1]
                        }
                    },
                    Group = groups[0]
                },
                new Project
                {
                    Name = "plan-alone",
                    Title = "Plan alone",
                    Description = "Mission management platform",
                    ProjectUsers = new List<ProjectUser>
                    {
                        new ProjectUser
                        {
                            User = users[1]
                        }
                    },
                    Group = groups[0]
                }
            };

            context.Projects.AddRange(projects);

            if (context.Processes.Any()) return;

            var processes = new List<Process>
            {
                new Process
                {
                    Title = "First Process",
                    Description = "Just a process",
                    IsDone = false,
                    Project = projects[0]
                },
                new Process
                {
                    Title = "Second Process",
                    Description = "Another process",
                    IsDone = true,
                    Project = projects[1]
                }
            };

            context.Processes.AddRange(processes);

            if (context.Missions.Any()) return;

            var missions = new List<Mission>
            {
                new Mission
                {
                    Title = "Drinking Water",
                    Description = "Just remind you to drink water :)",
                    State = MissionStates.New,
                    Priority = MissionPriorities.Medium,
                    CreateDate = DateTime.UtcNow,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CompletedDate = DateTime.UtcNow,
                    MissionUsers = new List<MissionUser>
                    {
                        new MissionUser
                        {
                            User = users[1]
                        }
                    },
                    Project = projects[2]
                },
                new Mission
                {
                    Title = "Run",
                    Description = "Run for your life",
                    State = MissionStates.New,
                    Priority = MissionPriorities.Medium,
                    CreateDate = DateTime.UtcNow,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CompletedDate = DateTime.UtcNow,
                    MissionUsers = new List<MissionUser>
                    {
                        new MissionUser
                        {
                            User = users[0]
                        },
                        new MissionUser
                        {
                            User = users[1]
                        }
                    },
                    Project = projects[1]
                },
                new Mission
                {
                    Title = "Taking a rest",
                    Description = "Just remind you to take a rest :)",
                    State = MissionStates.New,
                    Priority = MissionPriorities.Medium,
                    CreateDate = DateTime.UtcNow,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CompletedDate = DateTime.UtcNow,
                    MissionUsers = new List<MissionUser>
                    {
                        new MissionUser
                        {
                            User = users[0]
                        }
                    },
                    Project = projects[0]
                },
                new Mission
                {
                    Title = "Swimming",
                    Description = "Just swim",
                    State = MissionStates.New,
                    Priority = MissionPriorities.Medium,
                    CreateDate = DateTime.UtcNow,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CompletedDate = DateTime.UtcNow,
                    MissionUsers = new List<MissionUser>
                    {
                        new MissionUser
                        {
                            User = users[0]
                        }
                    },
                    Project = projects[1]
                },
            };

            context.Missions.AddRange(missions);

            var comments = new List<Comment>
            {
                new Comment
                {
                    Content = "What a mission!",
                    Mission = missions[1],
                    Owner = users[0]
                },
                new Comment
                {
                    Content = "O_o",
                    Mission = missions[1],
                    Owner = users[1]
                },
                new Comment
                {
                    Content = "Nice!",
                    Mission = missions[0],
                    Owner = users[1]
                }
            };

            context.Comments.AddRange(comments);

            context.SaveChanges();

        }
    }
}