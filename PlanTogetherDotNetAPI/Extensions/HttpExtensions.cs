using Newtonsoft.Json;
using PlanTogetherDotNetAPI.DTOs.Common;
using System.IO;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader value)
        {
            response.Headers.Add("pagination", JsonConvert.SerializeObject(value));
        }
    }
}