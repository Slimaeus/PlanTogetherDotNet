﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using PlanTogetherDotNetAPI.Data;
using PlanTogetherDotNetAPI.DTOs.Comments;
using PlanTogetherDotNetAPI.DTOs.Common;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class CommentsController : BaseApiController<Comment, CommentDTO>
    {
        public CommentsController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        // GET: api/Comments
        public IQueryable<CommentDTO> GetProjects([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Content.ToLower().Contains(@params.SearchTerm.ToLower())
            );

        // GET: api/Comments/5
        [ResponseType(typeof(CommentDTO))]
        public async Task<IHttpActionResult> GetComment(Guid id)
        {
            Comment comment = await Context.Comments
                .AsNoTracking()
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<CommentDTO>(comment));
        }

        // PUT: api/Comments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutComment(Guid id, EditCommentDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != input.Id)
            {
                return BadRequest();
            }
            var comment = await Context.Comments.FindAsync(id);
            Mapper.Map(input, comment);
            Context.Entry(comment).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        [ResponseType(typeof(CommentDTO))]
        public async Task<IHttpActionResult> PostComment(CreateCommentDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mission = await Context.Missions
                .FirstOrDefaultAsync(m => m.Id == input.MissionId);

            if (mission == null) return NotFound();

            var user = await Context.Users
                .FirstOrDefaultAsync(m => m.UserName == input.UserName);

            if (user == null) return NotFound();

            var comment = Mapper.Map<Comment>(input);

            comment.Mission = mission;

            comment.Owner = user;

            Context.Comments.Add(comment);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CommentExists(comment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = comment.Id }, Mapper.Map<CommentDTO>(comment));
        }

        // DELETE: api/Comments/5
        [ResponseType(typeof(CommentDTO))]
        public Task<IHttpActionResult> DeleteComment(Guid id)
            => base.Delete(id);

        private bool CommentExists(Guid id)
            => base.EntityExists(id);
    }
}