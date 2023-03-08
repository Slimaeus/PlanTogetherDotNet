using AutoMapper;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs.Processes;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.Models;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Web.Http;
using System;

namespace PlanTogetherDotNetAPI.Controllers.api
{
    public class ProcessesController : BaseApiController<Process, ProcessDTO>
    {
        public ProcessesController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<ProcessDTO> GetProjects([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Description.ToLower().Contains(@params.Query.ToLower())
            );
        [ResponseType(typeof(ProcessDTO))]
        public async Task<IHttpActionResult> GetProcess(Guid id)
        {
            Process process = await Context.Processes
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id);
            if (process == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<ProcessDTO>(process));
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProcess(Guid id, EditProcessDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != input.Id)
            {
                return BadRequest();
            }
            var process = await Context.Processes.FindAsync(id);
            Mapper.Map(input, process);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcessExists(id))
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
        [ResponseType(typeof(ProcessDTO))]
        public async Task<IHttpActionResult> PostProcess(CreateProcessDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await Context.Projects
                .FirstOrDefaultAsync(p => p.Name == input.ProjectName);

            if (project == null) return NotFound();

            var process = Mapper.Map<Process>(input);

            process.Project = project;

            Context.Processes.Add(process);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProcessExists(process.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = process.Id }, Mapper.Map<ProcessDTO>(process));
        }
        [ResponseType(typeof(ProcessDTO))]
        public Task<IHttpActionResult> DeleteProcess(Guid id)
            => base.Delete(id);
        private bool ProcessExists(Guid id)
            => base.EntityExists(id);
    }
}