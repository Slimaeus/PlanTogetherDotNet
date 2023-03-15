using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.Models;
using PlanTogetherDotNetAPI.Services;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;
using PlanTogetherDotNetAPI.Profiles;

namespace PlanTogetherDotNetAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<DataContext>(new HierarchicalLifetimeManager());

            container.RegisterType<TokenService>(new HierarchicalLifetimeManager());

            container.RegisterFactory<RoleStore<IdentityRole>>(
                c => new RoleStore<IdentityRole>(c.Resolve<DataContext>())
            );

            container.RegisterFactory<RoleManager<IdentityRole>>(
                c => new RoleManager<IdentityRole>(c.Resolve<RoleStore<IdentityRole>>())
            );

            container.RegisterFactory<UserStore<AppUser>>(
                c => new UserStore<AppUser>(c.Resolve<DataContext>())
            );
            container.RegisterFactory<UserManager<AppUser>>(
                c => new UserManager<AppUser>(c.Resolve<UserStore<AppUser>>())
            );

            //Config-AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            container.RegisterInstance(config.CreateMapper());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}