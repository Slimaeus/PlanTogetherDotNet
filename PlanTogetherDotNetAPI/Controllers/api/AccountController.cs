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
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, IMapper mapper, TokenService tokenService) 
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        [Authorize]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return BadRequest();
            var dto = _mapper.Map<UserDTO>(user);
            dto.Token = _tokenService.CreateToken(user);
            return Ok(dto);
        }
        [Authorize]
        [Route("user-count")]
        public async Task<IHttpActionResult> GetUserCountAsync()
        {
            var count = await _userManager.Users.CountAsync();
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
            var result = await _userManager.CreateAsync(user,input.Password);
            if(result.Succeeded)
            {
                var dto = _mapper.Map<UserDTO>(user);
                dto.Token = _tokenService.CreateToken(user);
                return Ok(dto);
            }
            return BadRequest();
        }
        [Route("login")]
        public async Task<IHttpActionResult> PostLoginAsync(LoginDTO lg)
        {
            var user = await _userManager.FindByNameAsync(lg.UserName);
            var result = await _userManager.CheckPasswordAsync(user,lg.Password);
            if(!result)
            {
                return Unauthorized();
            }
            var dto = _mapper.Map<UserDTO>(user);
            dto.Token = _tokenService.CreateToken(user);
            return Ok(dto);
        }
    }
}
