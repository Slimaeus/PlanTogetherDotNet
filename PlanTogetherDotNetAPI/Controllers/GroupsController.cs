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
using PlanTogetherDotNetAPI.DTOs.Group;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class GroupsController : ApiController
    {
        private readonly DataContext db;
        private readonly IMapper mapper;

        public GroupsController(DataContext context, IMapper mapper)
        {
            db = context;
            this.mapper = mapper;
        }
        // GET: api/Groups
        public IQueryable<GroupDTO> GetGroups()
        {
            return db.Groups.ProjectTo<GroupDTO>(mapper.ConfigurationProvider);
        }

        // GET: api/Groups/5
        [ResponseType(typeof(GroupDTO))]
        public async Task<IHttpActionResult> GetGroup(Guid id)
        {
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<GroupDTO>(group));
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

            var group = await db.Groups.FindAsync(id);

            mapper.Map(input, group);

            db.Entry(group).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
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

            var isNameTaken = await db.Groups
                .AnyAsync(g => g.Name == input.Name);

            if (isNameTaken)
            {

                ModelState.AddModelError(nameof(input.Name), "Name taken");
                return BadRequest(ModelState);
            }

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.UserName == input.UserName);

            if (user == null) return NotFound();

            var group = mapper.Map<Group>(input);

            group.Owner = user;

            db.Groups.Add(group);

            try
            {
                await db.SaveChangesAsync();
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

            return CreatedAtRoute("DefaultApi", new { id = group.Id }, mapper.Map<GroupDTO>(group));
        }

        // DELETE: api/Groups/5
        [ResponseType(typeof(GroupDTO))]
        public async Task<IHttpActionResult> DeleteGroup(Guid id)
        {
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(group);
            await db.SaveChangesAsync();

            return Ok(mapper.Map<GroupDTO>(group));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(Guid id)
        {
            return db.Groups.Count(e => e.Id == id) > 0;
        }
    }
}