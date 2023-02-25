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
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class MissionsController : ApiController
    {
        private readonly DataContext db;
        private readonly IMapper mapper;

        public MissionsController(DataContext context, IMapper mapper)
        {
            db = context;
            this.mapper = mapper;
        }
        // GET: api/Missions
        public IQueryable<MissionDTO> GetMissions()
        {
            //var missions = mapper.Map<IQueryable<MissionDTO>>(db.Missions);
            return db.Missions.ProjectTo<MissionDTO>(mapper.ConfigurationProvider);
        }

        // GET: api/Missions/5
        [ResponseType(typeof(MissionDTO))]
        public async Task<IHttpActionResult> GetMission(Guid id)
        {
            Mission mission = await db.Missions.FindAsync(id);
            MissionDTO missionDTO = mapper.Map<MissionDTO>(mission);
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
            var mission = await db.Missions.FindAsync(id);
            mapper.Map(input, mission);
            db.Entry(mission).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
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
        [ResponseType(typeof(Mission))]
        public async Task<IHttpActionResult> PostMission(CreateMissionDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var mission = mapper.Map<Mission>(input);
            db.Missions.Add(mission);

            try
            {
                await db.SaveChangesAsync();
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

            return CreatedAtRoute("DefaultApi", new { id = mission.Id }, mission);
        }

        // DELETE: api/Missions/5
        [ResponseType(typeof(Mission))]
        public async Task<IHttpActionResult> DeleteMission(Guid id)
        {
            Mission mission = await db.Missions.FindAsync(id);
            if (mission == null)
            {
                return NotFound();
            }

            db.Missions.Remove(mission);
            await db.SaveChangesAsync();

            return Ok(mission);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MissionExists(Guid id)
        {
            return db.Missions.Count(e => e.Id == id) > 0;
        }
    }
}