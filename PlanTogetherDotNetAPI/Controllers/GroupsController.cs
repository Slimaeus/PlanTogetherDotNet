using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Group;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class GroupsController : BaseApiController
    {
        public GroupsController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }
        // GET: api/Groups
        public IQueryable<GroupDTO> GetGroups([FromUri] PaginationParams @params)
        {
            if (@params.PageSize <= 0)
                return Context.Groups.AsNoTracking().ProjectTo<GroupDTO>(Mapper.ConfigurationProvider);

            var skipCount = ((@params.PageNumber > 1 ? @params.PageNumber : 1) - 1) * @params.PageSize;
            var takeCount = @params.PageSize;

            var query = Context.Groups.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(@params.SearchTerm))
            {
                query = query
                    .Where(m => m.Title.ToLower().Contains(@params.SearchTerm.ToLower())
                    || m.Description.ToLower().Contains(@params.SearchTerm.ToLower()));
            }

            return query
                .OrderBy(m => m.CreateDate)
                .Skip(skipCount)
                .Take(takeCount)
                .ProjectTo<GroupDTO>(Mapper.ConfigurationProvider);
        }

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
        public async Task<IHttpActionResult> DeleteGroup(Guid id)
        {
            Group group = await Context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            Context.Groups.Remove(group);
            await Context.SaveChangesAsync();

            return Ok(Mapper.Map<GroupDTO>(group));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(Guid id)
        {
            return Context.Groups.Count(e => e.Id == id) > 0;
        }
    }
}