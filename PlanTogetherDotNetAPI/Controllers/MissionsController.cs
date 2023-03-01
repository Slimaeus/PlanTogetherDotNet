using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class MissionsController : BaseApiController
    {
        public MissionsController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }
        // GET: api/Missions
        public IQueryable<MissionDTO> GetMissions([FromUri] PaginationParams @params)
        {
            if (@params.PageSize <= 0)
                return Context.Missions.AsNoTracking().ProjectTo<MissionDTO>(Mapper.ConfigurationProvider);

            var skipCount = ((@params.PageNumber > 1 ? @params.PageNumber : 1) - 1) * @params.PageSize;
            var takeCount = @params.PageSize;

            var query = Context.Missions.AsNoTracking().AsQueryable();

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
                .ProjectTo<MissionDTO>(Mapper.ConfigurationProvider);
        }

        // GET: api/Missions/5
        [ResponseType(typeof(MissionDTO))]
        public async Task<IHttpActionResult> GetMission(Guid id)
        {
            Mission mission = await Context.Missions
                .AsNoTracking()
                .Include(m => m.Comments)
                .Include(m => m.Comments.Select(c => c.Owner))
                .Include(m => m.MissionUsers)
                .Include(m => m.MissionUsers.Select(mu => mu.User))
                .FirstOrDefaultAsync(m => m.Id == id);
            MissionDTO missionDTO = Mapper.Map<MissionDTO>(mission);
            if (mission == null)
            {
                return NotFound();
            }

            return Ok(missionDTO);
        }

        // PUT: api/Missions/5
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
            Context.Entry(mission).State = EntityState.Modified;

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

        // POST: api/Missions
        [ResponseType(typeof(MissionDTO))]
        public async Task<IHttpActionResult> PostMission(CreateMissionDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var project = await Context.Projects
                .FirstOrDefaultAsync(p => p.Name == input.ProjectName);

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

        // DELETE: api/Missions/5
        [ResponseType(typeof(MissionDTO))]
        public async Task<IHttpActionResult> DeleteMission(Guid id)
        {
            Mission mission = await Context.Missions.FindAsync(id);
            if (mission == null)
            {
                return NotFound();
            }

            Context.Missions.Remove(mission);
            await Context.SaveChangesAsync();

            return Ok(Mapper.Map<MissionDTO>(mission));
        }

        private bool MissionExists(Guid id)
        {
            return Context.Missions.Count(e => e.Id == id) > 0;
        }
    }
}