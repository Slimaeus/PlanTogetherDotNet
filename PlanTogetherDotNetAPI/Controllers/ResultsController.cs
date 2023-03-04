using System;
using System.Web.Http;

namespace PlanTogetherDotNetAPI.Controllers
{
    [RoutePrefix("api/Results")]
    public class ResultsController : ApiController
    {
        [Route("null")]
        public IHttpActionResult GetNullException()
        {
            Exception n = null;
            return BadRequest(n.Message);
        }
        [Route("exception")]
        public IHttpActionResult GetException()
        {
            throw new Exception();
        }
        [Route("bad-request")]
        public IHttpActionResult GetBadRequest()
        {
            return BadRequest();
        }
        [Route("not-found")]
        public IHttpActionResult GetNotFound()
        {
            return NotFound();
        }
        [Route("unauthorized")]
        public IHttpActionResult GetUnauthorized()
        {
            return Unauthorized();
        }
    }
}
