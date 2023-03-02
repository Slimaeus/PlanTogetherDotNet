using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Group;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class GroupsController : BaseApiController<Group, GroupDTO>
    {
        public GroupsController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }
        // GET: api/Groups
        public IQueryable<GroupDTO> GetProjects([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Name.ToLower().Contains(@params.SearchTerm.ToLower()) || p.Title.Contains(@params.SearchTerm.ToLower())
            );

        // GET: api/Groups/5
        [ResponseType(typeof(GroupDTO))]
        public async Task<IHttpActionResult> GetGroup(Guid id)
        {
            Group group = await Context.Groups
                .AsNoTracking()
                .Include(g => g.Projects)
                .Include(g => g.Owner)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<GroupDTO>(group));
        }

        // PUT: api/Groups/5
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

        // POST: api/Groups
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
                .FirstOrDefaultAsync(u => u.UserName == input.UserName);

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

        // DELETE: api/Groups/5
        [ResponseType(typeof(GroupDTO))]
        public Task<IHttpActionResult> DeleteGroup(Guid id)
            => base.Delete(id);

        private bool GroupExists(Guid id)
            => base.EntityExists(id);
    }
}