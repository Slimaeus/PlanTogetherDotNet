using AutoMapper;
using PlanTogetherDotNetAPI.DTOs;
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
            CreateMap<CreateMissionDTO, Mission>();

            CreateMap<EditMissionDTO, Mission>()
                .ForAllMembers(options =>
                {
                    options.Condition((src, des, srcValue, desValue) => srcValue != null);
                });

            CreateMap<Mission, MissionDTO>();
        }
    }
}