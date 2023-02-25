using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.Models;
using PlanTogetherDotNetAPI.Profiles;
using System;
using System.Collections.Generic;
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
            var container = new UnityContainer(); //Dependency Injection

            container.RegisterType<DataContext>(new InjectionConstructor());
            container.RegisterFactory<UserManager<AppUser>>(
                c => new UserManager<AppUser>(new UserStore<AppUser>(new DataContext()))
            );

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            container.RegisterType<IMapper, Mapper>(new InjectionConstructor(config));

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
