using System;
using System.Web.Http;

namespace PlanTogetherDotNetAPI.Controllers
{
    public class ResultsController : ApiController
    {
        public IHttpActionResult GetNullException()
        {
            throw new ArrayTypeMismatchException("This is exception");
        }
    }
}
