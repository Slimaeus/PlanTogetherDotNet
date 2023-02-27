using AutoMapper;
using Microsoft.AspNet.Identity;
using PlanTogetherDotNetAPI.DTOs;
using PlanTogetherDotNetAPI.DTOs.Account;
using PlanTogetherDotNetAPI.Models;
using PlanTogetherDotNetAPI.Services;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly TokenService tokenService;

        public AccountController(UserManager<AppUser> userManager, IMapper mapper, TokenService tokenService) 
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.tokenService = tokenService;
        }

        public IHttpActionResult GetCurrentUser()
        {
            return Ok();
        }

        [Authorize]
        [Route("user-count")]
        public async Task<IHttpActionResult> GetUserCountAsync()
        {
            var count = await userManager.Users.CountAsync();
            return Ok(count);
        }

        [Route("register")]
        public async Task<IHttpActionResult> PostRegisterAsync(RegisterDTO input)
        {
            var user = new AppUser
            {
                UserName = input.UserName,
                Email = input.Email,
                DisplayName = input.DisplayName
            };
            var result = await userManager.CreateAsync(user,input.Password);
            if(result.Succeeded)
            {
                var dto = mapper.Map<UserDTO>(user);
                dto.Token = tokenService.CreateToken(user);
                return Ok(dto);
            }
            return BadRequest();
        }

        [Route("login")]
        public async Task<IHttpActionResult> PostLoginAsync(LoginDTO lg)
        {
            var user = await userManager.FindByNameAsync(lg.UserName);
            var result = await userManager.CheckPasswordAsync(user,lg.Password);
            if(!result)
            {
                return Unauthorized();
            }
            var dto = mapper.Map<UserDTO>(user);
            dto.Token = tokenService.CreateToken(user);
            return Ok(dto);
        }
    }
}
