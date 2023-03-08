using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Group;
using PlanTogetherDotNetAPI.DTOs.Project;
using PlanTogetherDotNetAPI.Extensions;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class GroupsController : BaseApiController<Group, GroupDTO>
    {
        public GroupsController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<GroupDTO> GetProjects([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Name.ToLower().Contains(@params.Query.ToLower()) || p.Title.Contains(@params.Query.ToLower())
            );
        [ResponseType(typeof(GroupDTO))]
        public async Task<IHttpActionResult> GetGroup(Guid id)
        {
            Group group = await Context.Groups
                .AsNoTracking()
                .Include(g => g.Projects)
                .Include(g => g.Owner)
                .SingleOrDefaultAsync(g => g.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<GroupDTO>(group));
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGroup(Guid id, EditGroupDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != input.Id)
            {
                return BadRequest();
            }

            var group = await Context.Groups.FindAsync(id);

            Mapper.Map(input, group);

            Context.Entry(group).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        [ResponseType(typeof(GroupDTO))]
        public async Task<IHttpActionResult> PostGroup(CreateGroupDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isNameTaken = await Context.Groups
                .AnyAsync(g => g.Name == input.Name);

            if (isNameTaken)
            {

                ModelState.AddModelError(nameof(input.Name), "Name taken");
                return BadRequest(ModelState);
            }

            var user = await Context.Users
                .SingleOrDefaultAsync(u => u.UserName == input.UserName);

            if (user == null) return NotFound();

            var group = Mapper.Map<Group>(input);

            group.Owner = user;

            Context.Groups.Add(group);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GroupExists(group.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = group.Id }, Mapper.Map<GroupDTO>(group));
        }
        [ResponseType(typeof(GroupDTO))]
        public Task<IHttpActionResult> DeleteGroup(Guid id)
            => base.Delete(id);
        [ResponseType(typeof(ProjectDTO))]
        [Route("{name}/projects")]
        public IQueryable<ProjectDTO> GetProjects(string name, [FromUri(Name = "")] PaginationParams @params)
        {
            var query = Context.Projects
                .AsNoTracking()
                .Where(g => g.Group.Name == name);

            if (!string.IsNullOrEmpty(@params.Query))
                query = query.Where(g => g.Title.ToLower().Contains(@params.Query)
                || g.Description.ToLower().Contains(@params.Query)
                || g.Name.ToLower().Contains(@params.Query));

            var count = query.Count();

            query = query.Paginate(@params.Index, @params.Size);
            HttpContext.Current.Response.AddPaginationHeader(new PaginationHeader(@params.Index, @params.Size, count));
            return query
                .ProjectTo<ProjectDTO>(Mapper.ConfigurationProvider);
        }
        private bool GroupExists(Guid id)
            => base.EntityExists(id);
    }
}