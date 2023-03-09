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
    public class ProcessesController : BaseApiController<Process, ProcessDTO, EditProcessDTO>
    {
        public ProcessesController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<ProcessDTO> GetProjects([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Description.ToLower().Contains(@params.Query.ToLower())
            );
        [ResponseType(typeof(ProcessDTO))]
        public Task<IHttpActionResult> GetProcess(Guid id)
            => Get(id);
        [ResponseType(typeof(void))]
        [Route("{id:guid}")]
        public Task<IHttpActionResult> PutProcess(Guid id, EditProcessDTO input)
            => Put(id, input);
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
            => Delete(id);
        private bool ProcessExists(Guid id)
            => EntityExists(id);
    }
}