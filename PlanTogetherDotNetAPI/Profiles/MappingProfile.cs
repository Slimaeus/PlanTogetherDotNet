﻿using AutoMapper;
using PlanTogetherDotNetAPI.DTOs;
using PlanTogetherDotNetAPI.DTOs.Account;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Group;
using PlanTogetherDotNetAPI.DTOs.Project;
using PlanTogetherDotNetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Mission
            CreateMap<Mission, MissionDTO>();
            CreateMap<CreateMissionDTO, Mission>();
            CreateMap<EditMissionDTO, Mission>()
                .ForAllMembers(options =>
                {
                    options.Condition((src, des, srcValue, desValue) => srcValue != null);
                });
            #endregion

            #region Project
            CreateMap<Project, ProjectDTO>();
            CreateMap<CreateProjectDTO, Project>();
            CreateMap<EditProjectDTO, Project>()
                .ForAllMembers(options =>
                {
                    options.Condition((src, des, srcValue, desValue) => srcValue != null);
                });
            #endregion

            #region Group
            CreateMap<Group, GroupDTO>();

            CreateMap<CreateGroupDTO, Group>();

            CreateMap<EditGroupDTO, Group>()
                .ForAllMembers(options =>
                {
                    options.Condition((src, des, srcValue, desValue) => srcValue != null);
                });
            #endregion

            #region User
            CreateMap<AppUser, UserDTO>();
            CreateMap<AppUser, MemberDTO>();
            #endregion
        }
    }
}