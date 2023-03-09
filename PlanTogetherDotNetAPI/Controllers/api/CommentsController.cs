using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
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
    public class CommentsController : BaseApiController<Comment, CommentDTO, EditCommentDTO>
    {
        public CommentsController(DataContext context, IMapper mapper) : base(context, mapper) {}
        public IQueryable<CommentDTO> GetComments([FromUri(Name = "")] PaginationParams @params)
            => Get(
                @params, p => p.Content.ToLower().Contains(@params.Query.ToLower())
            );
        [ResponseType(typeof(CommentDTO))]
        public Task<IHttpActionResult> GetComment(Guid id)
            => Get(id);
        [ResponseType(typeof(void))]
        [Route("{id:guid}")]
        public Task<IHttpActionResult> PutComment(Guid id, EditCommentDTO input)
            => Put(id, input);
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
        [ResponseType(typeof(CommentDTO))]
        public Task<IHttpActionResult> DeleteComment(Guid id)
            => Delete(id);
        private bool CommentExists(Guid id)
            => EntityExists(id);
    }
}