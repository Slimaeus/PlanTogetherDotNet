using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Owin;
using PlanTogetherDotNetAPI.Middlewares;
using System.Configuration;
using System.Text;

[assembly: OwinStartup(typeof(PlanTogetherDotNetAPI.Startup))]
namespace PlanTogetherDotNetAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["TokenKey"]));

            var options = new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = false
                }
            };
            app.UseJwtBearerAuthentication(options);
            //app.Use(typeof(ExceptionMiddleware));
        }
    }
}
