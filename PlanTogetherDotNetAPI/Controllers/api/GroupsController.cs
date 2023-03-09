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
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Group;
using PlanTogetherDotNetAPI.DTOs.Project;
using PlanTogetherDotNetAPI.Extensions;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Groups")]
    public class GroupsController : BaseApiController<Group, GroupDTO, EditGroupDTO>
    {
        public GroupsController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<GroupDTO> GetGroups([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Name.ToLower().Contains(@params.Query.ToLower())
                || p.Title.Contains(@params.Query.ToLower())
            );
        [ResponseType(typeof(GroupDTO))]
        [Route("{id:guid}")]
        public Task<IHttpActionResult> GetGroup(Guid id)
            => Get(id);
        [ResponseType(typeof(GroupDTO))]
        [Route("{name}")]
        public async Task<IHttpActionResult> GetGroupByName(string name)
        {
            GroupDTO groupDTO = await Context.Groups
                .AsNoTracking()
                .ProjectTo<GroupDTO>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Name == name);
            if (groupDTO == null)
            {
                return NotFound();
            }
            return Ok(groupDTO);
        }
        [ResponseType(typeof(void))]
        [Route("{id:guid}")]
        public Task<IHttpActionResult> PutGroup(Guid id, EditGroupDTO input)
            => Put(id, input);
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
        [Route("{id:guid}")]
        public Task<IHttpActionResult> DeleteGroup(Guid id)
            => Delete(id);
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
            => EntityExists(id);
    }
}