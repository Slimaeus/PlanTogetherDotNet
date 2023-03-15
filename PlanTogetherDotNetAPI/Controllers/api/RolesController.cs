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
        public IQueryable<IdentityRole> GetRoles()
            => _roleManager.Roles;
        public async Task<IHttpActionResult> PostRole(string name)
        {
            var isExists = await _roleManager.RoleExistsAsync(name);
            if (isExists)
            {
                ModelState.AddModelError("role", "Role already existed");
                return BadRequest(ModelState);
            }
            var role = new IdentityRole { Name = name };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded) return StatusCode(System.Net.HttpStatusCode.NoContent);
            return InternalServerError();
        }
        public async Task<IHttpActionResult> DeleteRole(string name)
        {
            var role = await _roleManager.FindByIdAsync(name);
            if (role == null)
            {
                ModelState.AddModelError("role", "Role did not exist");
                return BadRequest(ModelState);
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded) return StatusCode(System.Net.HttpStatusCode.NoContent);
            return InternalServerError();
        }
        [Authorize(Roles = "Admin")]
        [Route("add-role/{username}/{role}")]
        public async Task<IHttpActionResult> GetAddAdmin(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound();
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return NotFound();
            await _userManager.AddToRoleAsync(user.Id, role.Name);
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
        [Authorize(Roles = "Admin")]
        [Route("admin-test")]
        public IHttpActionResult GetAdminTest()
        {
            return Ok("You have Admin role o_O");
        }
        [Authorize]
        [Route("get-current-user-roles")]
        public IHttpActionResult GetUserRoles()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.FindAll(ClaimTypes.Role).Select(c => c.Value);
            return Ok(roles);
        }
    }
}
