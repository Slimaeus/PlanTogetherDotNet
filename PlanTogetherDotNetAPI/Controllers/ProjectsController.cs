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
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Project;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Projects")]
    public class ProjectsController : BaseApiController
    {
        public ProjectsController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        // GET: api/Projects
        public IQueryable<ProjectDTO> GetProjects([FromUri] PaginationParams @params)
        {
            if (@params.PageSize <= 0)
                return Context.Projects.AsNoTracking().ProjectTo<ProjectDTO>(Mapper.ConfigurationProvider);

            var skipCount = ((@params.PageNumber > 1 ? @params.PageNumber : 1) - 1) * @params.PageSize;
            var takeCount = @params.PageSize;

            var query = Context.Projects.AsNoTracking().AsQueryable();

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
                .ProjectTo<ProjectDTO>(Mapper.ConfigurationProvider);
        }

        // GET: api/Projects/5
        [ResponseType(typeof(ProjectDTO))]
        public async Task<IHttpActionResult> GetProject(Guid id)
        {
            Project project = await Context.Projects
                .AsNoTracking()
                .Include(p => p.Missions)
                .FirstOrDefaultAsync(p => p.Id == id);
            ProjectDTO projectDTO = Mapper.Map<ProjectDTO>(project);
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

            var project = await Context.Projects.FindAsync(id);
            Mapper.Map(input, project);
            Context.Entry(project).State = EntityState.Modified;

            if (id != project.Id)
            {
                return BadRequest();
            }

            Context.Entry(project).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
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
            
            var isNameTaken = await Context.Projects
                .AnyAsync(p => p.Name == input.Name);

            if (isNameTaken)
            {
                ModelState.AddModelError(nameof(input.Name), "Name taken");
                return BadRequest(ModelState);
            }

            var group = await Context.Groups
                .FirstOrDefaultAsync(g => g.Name == input.GroupName);

            if (group == null) return NotFound();

            var project = Mapper.Map<Project>(input);
            project.Group = group;
            Context.Projects.Add(project);

            try
            {
                await Context.SaveChangesAsync();
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

            return CreatedAtRoute("DefaultApi", new { id = project.Id }, Mapper.Map<ProjectDTO>(project));
        }

        // DELETE: api/Projects/5
        [ResponseType(typeof(ProjectDTO))]
        public async Task<IHttpActionResult> DeleteProject(Guid id)
        {
            Project project = await Context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            Context.Projects.Remove(project);
            await Context.SaveChangesAsync();

            return Ok(Mapper.Map<ProjectDTO>(project));
        }

        [Route("{projectId}/add-mission/{missionId}")]
        public async Task<IHttpActionResult> PatchAddMission(Guid projectId, Guid missionId)
        {
            var project = await Context.Projects
                .Include(p => p.Missions)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return NotFound();

            var isMissionExists = project.Missions
                .Any(m => m.Id == missionId);

            if (isMissionExists) return BadRequest();

            var mission = await Context.Missions
                .FirstOrDefaultAsync(m => m.Id == missionId);

            if (mission == null) return NotFound();

            project.Missions.Add(mission);

            var success = await Context.SaveChangesAsync() > 0;

            if (success) return Ok();

            return BadRequest();
        }

        private bool ProjectExists(Guid id)
        {
            return Context.Projects.Count(e => e.Id == id) > 0;
        }
    }
}