﻿using PlanTogetherDotNetAPI.Enums;
using PlanTogetherDotNetAPI.Models;
using System.Collections.Generic;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;

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

            if (context.Users.Any()) return;

            var users = new List<AppUser>
            {
                new AppUser
                {
                    DisplayName = "thai",
                    UserName = "thai",
                    Email = "thai@test.com"
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
                userManager.Create(user, "P@ssw0rd");
            }

            if (context.Projects.Any()) return;

            var projects = new List<Project>
                        {
                            new Project
                            {
                                Name = "caro-online",
                                Title = "Caro Online",
                                Description = "Caro game",
                                //Members = new List<ProjectUser>
                                //{
                                //    new ProjectUser
                                //    {
                                //        User = users[0]
                                //    }
                                //},
                                //Missions = new List<Mission>
                                //{
                                //    missions[2]
                                //},
                                //Processes = new List<Process>
                                //{
                                //    processes[0]
                                //}
                            },
                            new Project
                            {
                                Name = "plan-together",
                                Title = "Plan together",
                                Description = "Mission management platform",
                                //Members = new List<ProjectUser>
                                //{
                                //    new ProjectUser
                                //    {
                                //        User = users[0]
                                //    },
                                //    new ProjectUser
                                //    {
                                //        User = users[1]
                                //    }
                                //},
                                //Missions = new List<Mission>
                                //{
                                //    missions[1],
                                //    missions[3]
                                //},
                                //Processes = new List<Process>
                                //{
                                //    processes[1]
                                //}
                            },
                            new Project
                            {
                                Name = "plan-alone",
                                Title = "Plan alone",
                                Description = "Mission management platform",
                                //Members = new List<ProjectUser>
                                //{
                                //    new ProjectUser
                                //    {
                                //        User = users[1]
                                //    }
                                //},
                                //Missions = new List<Mission>
                                //{
                                //    missions[0]
                                //}
                            }
                        };

            context.Projects.AddRange(projects);


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

            context.SaveChanges();

        }
    }
}