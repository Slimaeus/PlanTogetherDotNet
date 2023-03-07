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

namespace PlanTogetherDotNetAPI.Controllers
{
    public abstract class BaseApiController<TEntity, TDTO> : ApiController
        where TEntity : Entity
        where TDTO : class
    {
        public BaseApiController(DataContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
        protected DataContext Context { get; }
        protected IMapper Mapper { get; }
        protected virtual IQueryable<TDTO> Get(PaginationParams @params, Expression<Func<TEntity, bool>> predicate)
        {
            var query = Context.Set<TEntity>()
                .AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(@params.SearchTerm))
            {
                query = query
                    .Where(predicate);
            }

            var count = query.Count();

            query = query.Paginate(@params.PageNumber, @params.PageSize);

            HttpContext.Current.Response.AddPaginationHeader(new PaginationHeader(@params.PageNumber, @params.PageSize, count));
            return query
                .ProjectTo<TDTO>(Mapper.ConfigurationProvider);
        }
        public async Task<IHttpActionResult> Delete(Guid id)
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
        public bool EntityExists(Guid id)
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
