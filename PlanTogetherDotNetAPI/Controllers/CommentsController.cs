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
using PlanTogetherDotNetAPI.DTOs.Comments;
using PlanTogetherDotNetAPI.Models;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class CommentsController : ApiController
    {
        private readonly DataContext db;
        private readonly IMapper mapper;

        public CommentsController(DataContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }


        // GET: api/Comments
        public IQueryable<CommentDTO> GetComments()
        {
            return db.Comments.ProjectTo<CommentDTO>(mapper.ConfigurationProvider);
        }

        // GET: api/Comments/5
        [ResponseType(typeof(CommentDTO))]
        public async Task<IHttpActionResult> GetComment(Guid id)
        {
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CommentDTO>(comment));
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
            var comment = await db.Comments.FindAsync(id);
            mapper.Map(input, comment);
            db.Entry(comment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
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

            var mission = await db.Missions
                .FirstOrDefaultAsync(m => m.Id == input.MissionId);

            if (mission == null) return NotFound();

            var user = await db.Users
                .FirstOrDefaultAsync(m => m.UserName == input.UserName);

            if (user == null) return NotFound();

            var comment = mapper.Map<Comment>(input);

            comment.Mission = mission;
            
            comment.Owner = user;

            db.Comments.Add(comment);

            try
            {
                await db.SaveChangesAsync();
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

            return CreatedAtRoute("DefaultApi", new { id = comment.Id }, mapper.Map<CommentDTO>(comment));
        }

        // DELETE: api/Comments/5
        [ResponseType(typeof(CommentDTO))]
        public async Task<IHttpActionResult> DeleteComment(Guid id)
        {
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.Comments.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(mapper.Map<CommentDTO>(comment));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentExists(Guid id)
        {
            return db.Comments.Count(e => e.Id == id) > 0;
        }
    }
}