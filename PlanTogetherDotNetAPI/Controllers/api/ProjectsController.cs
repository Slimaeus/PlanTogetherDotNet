﻿using System;
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
using PlanTogetherDotNetAPI.DTOs.Project;
using PlanTogetherDotNetAPI.Extensions;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Projects")]
    public class ProjectsController : BaseApiController<Project, ProjectDTO>
    {
        public ProjectsController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<ProjectDTO> GetProjects([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Name.ToLower().Contains(@params.Query.ToLower()) || p.Title.Contains(@params.Query.ToLower())
            );
        [ResponseType(typeof(ProjectDTO))]
        public async Task<IHttpActionResult> GetProject(Guid id)
        {
            Project project = await Context.Projects
                .AsNoTracking()
                .Include(p => p.Missions)
                .SingleOrDefaultAsync(p => p.Id == id);
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
                .SingleOrDefaultAsync(g => g.Name == input.GroupName);

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
        [ResponseType(typeof(MissionDTO))]
        [Route("{id}/missions")]
        public IQueryable<MissionDTO> GetMissions(Guid id, [FromUri(Name = "")] PaginationParams @params)
        {
            var query = Context.Missions
                .AsNoTracking()
                .Where(m => m.ProjectId == id);

            if (!string.IsNullOrEmpty(@params.Query))
                query = query.Where(m => m.Title.ToLower().Contains(@params.Query) || m.Description.ToLower().Contains(@params.Query));

            var count = query.Count();

            query = query.Paginate(@params.Index, @params.Size);
            HttpContext.Current.Response.AddPaginationHeader(new PaginationHeader(@params.Index, @params.Size, count));
            return query
                .ProjectTo<MissionDTO>(Mapper.ConfigurationProvider);
        }
        [Route("{name}/add-member/{username}")]
        public async Task<IHttpActionResult> PatchAddMember(string name, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await Context.Users
                .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null) return NotFound();

            var project = await Context.Projects
                .Include(m => m.ProjectUsers)
                .SingleOrDefaultAsync(m => m.Name == name);

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
                .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null) return NotFound();

            var project = await Context.Projects
                .Include(m => m.ProjectUsers)
                .SingleOrDefaultAsync(m => m.Name == name);

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