using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.DTOs;
using PlanTogetherDotNetAPI.Extensions;
using PlanTogetherDotNetAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Linq.Expressions;
using Newtonsoft.Json;
using PlanTogetherDotNetAPI.DTOs.Comments;

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

        protected virtual IQueryable<TDTO> Get([FromUri(Name = "")] PaginationParams @params, Expression<Func<TEntity, bool>> predicate)
        {
            if (@params.PageSize <= 0)
                return Context.Set<TEntity>().AsNoTracking().ProjectTo<TDTO>(Mapper.ConfigurationProvider);

            var query = Context.Set<TEntity>()
                .AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(@params.SearchTerm))
            {
                query = query
                    .Where(predicate);
            }

            query = query.Paginate(@params.PageNumber, @params.PageSize);

            HttpContext.Current.Response.AddPaginationHeader(new PaginationHeader(@params.PageNumber, @params.PageSize, Context.Set<TEntity>().Count()));
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
