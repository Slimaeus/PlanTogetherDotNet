using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PlanTogetherDotNetAPI.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PlanTogetherDotNetAPI.Controllers.api
{
    [RoutePrefix("api/Roles")]
    public class RolesController : ApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]
        [Route("add-amdmin")]
        public async Task<IHttpActionResult> GetAddAdmin(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null) return BadRequest();
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null) return BadRequest();
            await _userManager.AddToRoleAsync(user.Id, adminRole.Name);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [Route("admin-test")]
        public async Task<IHttpActionResult> GetAdminTest()
        {
            return Ok("You have Admin role o_O");
        }
        [Authorize]
        public async Task<IHttpActionResult> GetRoles()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.FindAll(ClaimTypes.Role).Select(c => c.Value);
            return Ok(roles);
        }
    }
}
