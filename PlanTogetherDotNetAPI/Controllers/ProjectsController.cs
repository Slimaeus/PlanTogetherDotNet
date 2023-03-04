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
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs.Project;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Projects")]
    public class ProjectsController : BaseApiController<Project, ProjectDTO>
    {
        public ProjectsController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<ProjectDTO> GetProjects([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Name.ToLower().Contains(@params.SearchTerm.ToLower()) || p.Title.Contains(@params.SearchTerm.ToLower())
            );
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
        [ResponseType(typeof(ProjectDTO))]
        public Task<IHttpActionResult> DeleteProject(Guid id)
            => base.Delete(id);

        [Route("{name}/add-member/{username}")]
        public async Task<IHttpActionResult> PatchAddMember(string name, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await Context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null) return NotFound();

            var project = await Context.Projects
                .Include(m => m.ProjectUsers)
                .FirstOrDefaultAsync(m => m.Name == name);

            if (project == null) return NotFound();

            var member = project.ProjectUsers.FirstOrDefault(m => m.UserId == user.Id);

            if (member != null)
            {
                ModelState.AddModelError("Member", "User already added");
                return BadRequest(ModelState);
            }

            project.ProjectUsers.Add(new ProjectUser { User = user });

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.Id))
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
        [Route("{name}/remove-member/{username}")]
        public async Task<IHttpActionResult> PatchRemoveMember(string name, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await Context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null) return NotFound();

            var project = await Context.Projects
                .Include(m => m.ProjectUsers)
                .FirstOrDefaultAsync(m => m.Name == name);

            if (project == null) return NotFound();

            var member = project.ProjectUsers.FirstOrDefault(m => m.UserId == user.Id);

            if (member == null)
            {
                ModelState.AddModelError("Member", "User did not add");
                return BadRequest(ModelState);
            }

            project.ProjectUsers.Remove(member);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.Id))
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
        private bool ProjectExists(Guid id)
            => base.EntityExists(id);
    }
}