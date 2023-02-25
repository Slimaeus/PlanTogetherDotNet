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
using PlanTogetherDotNetAPI.DTOs.Project;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Projects")]
    public class ProjectsController : ApiController
    {
        //private DataContext db = new DataContext();
        private readonly DataContext db;
        private readonly IMapper mapper;

        public ProjectsController(DataContext context, IMapper mapper) 
        {
            db = context;
            this.mapper = mapper;
        }

        // GET: api/Projects
        public IQueryable<ProjectDTO> GetProjects()
        {
            return db.Projects.ProjectTo<ProjectDTO>(mapper.ConfigurationProvider);
        }

        // GET: api/Projects/5
        [ResponseType(typeof(ProjectDTO))]
        public async Task<IHttpActionResult> GetProject(Guid id)
        {
            Project project = await db.Projects.FindAsync(id);
            ProjectDTO projectDTO = mapper.Map<ProjectDTO>(project);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(projectDTO);
        }

        // PUT: api/Projects/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProject(Guid id, EditProjectDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await db.Projects.FindAsync(id);
            mapper.Map(input, project);
            db.Entry(project).State = EntityState.Modified;

            if (id != project.Id)
            {
                return BadRequest();
            }

            db.Entry(project).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        [ResponseType(typeof(ProjectDTO))]
        public async Task<IHttpActionResult> PostProject(CreateProjectDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = await db.Groups
                .Include(g => g.Projects)
                .FirstOrDefaultAsync(g => g.Name == input.GroupName);

            if (group == null) return NotFound();

            var project = mapper.Map<Project>(input);
            group.Projects.Add(project);
            db.Projects.Add(project);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProjectExists(project.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = project.Id }, mapper.Map<ProjectDTO>(project));
        }

        // DELETE: api/Projects/5
        [ResponseType(typeof(ProjectDTO))]
        public async Task<IHttpActionResult> DeleteProject(Guid id)
        {
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            db.Projects.Remove(project);
            await db.SaveChangesAsync();

            return Ok(mapper.Map<ProjectDTO>(project));
        }

        [Route("{projectId}/add-mission/{missionId}")]
        public async Task<IHttpActionResult> PatchAddMission(Guid projectId, Guid missionId)
        {
            var project = await db.Projects
                .Include(p => p.Missions)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return NotFound();

            var isMissionExists = project.Missions
                .Any(m => m.Id == missionId);

            if (isMissionExists) return BadRequest();

            var mission = await db.Missions
                .FirstOrDefaultAsync(m => m.Id == missionId);

            if (mission == null) return NotFound();

            project.Missions.Add(mission);

            var success = await db.SaveChangesAsync() > 0;

            if (success) return Ok();

            return BadRequest();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectExists(Guid id)
        {
            return db.Projects.Count(e => e.Id == id) > 0;
        }
    }
}