using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.Models;
using PlanTogetherDotNetAPI.Profiles;
using PlanTogetherDotNetAPI.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Unity.Injection;
using Unity.WebApi;

namespace PlanTogetherDotNetAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new UnityContainer(); //Init-Dependency-Injection

            container.RegisterType<DataContext>(new InjectionConstructor());

            container.RegisterType<TokenService>(new InjectionConstructor());

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

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container); //Config-Dependency-Injection

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
