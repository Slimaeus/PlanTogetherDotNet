using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.Extensions;
using PlanTogetherDotNetAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;

namespace PlanTogetherDotNetAPI.Controllers
{
    public abstract class BaseApiController<TEntity, TDTO, TEditDTO> : ApiController
        where TEntity : Entity
        where TDTO : EntityDTO
        where TEditDTO : EditDTO
    {
        public BaseApiController(DataContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
        protected DataContext Context { get; }
        protected IMapper Mapper { get; }
        protected IQueryable<TDTO> Get(PaginationParams @params, Expression<Func<TEntity, bool>> predicate)
        {
            var query = Context.Set<TEntity>()
                .AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(@params.Query))
            {
                query = query
                    .Where(predicate);
            }

            var count = query.Count();

            query = query.Paginate(@params.Index, @params.Size);

            HttpContext.Current.Response.AddPaginationHeader(new PaginationHeader(@params.Index, @params.Size, count));
            return query
                .ProjectTo<TDTO>(Mapper.ConfigurationProvider);
        }
        protected async Task<IHttpActionResult> Get(Guid id)
        {
            TDTO entityDTO = await Context.Set<TEntity>()
                .AsNoTracking()
                .ProjectTo<TDTO>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(g => g.Id == id);
            if (entityDTO == null)
            {
                return NotFound();
            }

            return Ok(entityDTO);
        }
        protected async Task<IHttpActionResult> Put(Guid id, TEditDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != input.Id)
            {
                return BadRequest();
            }
            var entity = await Context.Set<TEntity>().FindAsync(id);
            Mapper.Map(input, entity);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
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
        protected async Task<IHttpActionResult> Delete(Guid id)
        {
            TEntity entity = await Context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            Context.Set<TEntity>().Remove(entity);
            await Context.SaveChangesAsync();

            return Ok(Mapper.Map<TDTO>(entity));
        }
        protected bool EntityExists(Guid id)
        {
            return Context.Set<TEntity>().Count(e => e.Id == id) > 0;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
