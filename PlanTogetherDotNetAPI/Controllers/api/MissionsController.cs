using System;
using System.Data;
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
using PlanTogetherDotNetAPI.DTOs.Comments;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.Extensions;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Missions")]
    public class MissionsController : BaseApiController<Mission, MissionDTO>
    {
        public MissionsController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<MissionDTO> GetMissions([FromUri(Name = "")] PaginationParams @params)
            => base.Get(@params, m => m.Title.ToLower().Contains(@params.Query.ToLower()) || m.Description.Contains(@params.Query.ToLower()));
        [ResponseType(typeof(MissionDTO))]
        public async Task<IHttpActionResult> GetMission(Guid id)
        {
            Mission mission = await Context.Missions
                .AsNoTracking()
                .Include(m => m.Comments)
                .Include(m => m.Comments.Select(c => c.Owner))
                .Include(m => m.MissionUsers)
                .Include(m => m.MissionUsers.Select(mu => mu.User))
                .SingleOrDefaultAsync(m => m.Id == id);
            MissionDTO missionDTO = Mapper.Map<MissionDTO>(mission);
            if (mission == null)
            {
                return NotFound();
            }

            return Ok(missionDTO);
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMission(Guid id, EditMissionDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != input.Id)
            {
                return BadRequest();
            }
            var mission = await Context.Missions.FindAsync(id);
            Mapper.Map(input, mission);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissionExists(id))
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
        [ResponseType(typeof(MissionDTO))]
        public async Task<IHttpActionResult> PostMission(CreateMissionDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var project = await Context.Projects
                .SingleOrDefaultAsync(p => p.Name == input.ProjectName);

            if (project == null) return NotFound();

            var mission = Mapper.Map<Mission>(input);
            mission.Project = project;
            Context.Missions.Add(mission);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MissionExists(mission.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = mission.Id }, Mapper.Map<MissionDTO>(mission));
        }
        [ResponseType(typeof(MissionDTO))]
        public Task<IHttpActionResult> DeleteMission(Guid id)
            => base.Delete(id);
        [ResponseType(typeof(MemberDTO))]
        [Route("{id}/members")]
        public IQueryable<MemberDTO> GetMembers(Guid id, [FromUri(Name = "")] PaginationParams @params)
        {
            var query = Context.MissionUsers
                .AsNoTracking()
                .Where(pu => pu.MissionId == id)
                .Select(pu => pu.User);

            if (!string.IsNullOrEmpty(@params.Query))
                query = query.Where(u => u.UserName.ToLower().Contains(@params.Query) || u.Email.ToLower().Contains(@params.Query));

            var count = query.Count();

            query = query.UserPaginate(@params.Index, @params.Size);
            HttpContext.Current.Response.AddPaginationHeader(new PaginationHeader(@params.Index, @params.Size, count));
            return query
                .ProjectTo<MemberDTO>(Mapper.ConfigurationProvider);
        }
        [ResponseType(typeof(CommentDTO))]
        [Route("{id}/comments")]
        public IQueryable<CommentDTO> GetComments(Guid id, [FromUri(Name = "")] PaginationParams @params)
        {
            var query = Context.Comments
                .AsNoTracking()
                .Where(c => c.MissionId == id);

            if (!string.IsNullOrEmpty(@params.Query))
            {
                query = query
                    .Where(p => p.Content.ToLower().Contains(@params.Query.ToLower()));
            }

            var count = query.Count();

            query = query.Paginate(@params.Index, @params.Size);

            HttpContext.Current.Response.AddPaginationHeader(new PaginationHeader(@params.Index, @params.Size, count));
            return query
                .ProjectTo<CommentDTO>(Mapper.ConfigurationProvider);
        }
        [Route("{id}/add-member/{username}")]
        public async Task<IHttpActionResult> PatchAddMember(Guid id, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await Context.Users
                .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null) return NotFound();

            var mission = await Context.Missions
                .Include(m => m.MissionUsers)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (mission == null) return NotFound();

            var member = mission.MissionUsers.FirstOrDefault(m => m.UserId == user.Id);

            if (member != null)
            {
                ModelState.AddModelError("Member", "User already added");
                return BadRequest(ModelState);
            }

            mission.MissionUsers.Add(new MissionUser { User = user });

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissionExists(id))
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
        [Route("{id}/remove-member/{username}")]
        public async Task<IHttpActionResult> PatchRemoveMember(Guid id, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await Context.Users
                .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null) return NotFound();

            var mission = await Context.Missions
                .Include(m => m.MissionUsers)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (mission == null) return NotFound();

            var member = mission.MissionUsers.FirstOrDefault(m => m.UserId == user.Id);

            if (member == null)
            {
                ModelState.AddModelError("Member", "User did not add");
                return BadRequest(ModelState);
            }

            mission.MissionUsers.Remove(member);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissionExists(id))
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
        private bool MissionExists(Guid id)
            => base.EntityExists(id);
    }
}