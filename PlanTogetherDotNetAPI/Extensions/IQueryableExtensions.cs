using PlanTogetherDotNetAPI.Models;
using System.Linq;

namespace PlanTogetherDotNetAPI.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
            where T : Entity
        {
            var skipCount = ((pageNumber > 1 ? pageNumber : 1) - 1) * pageSize;
            var takeCount = pageSize;

            return query
                .OrderBy(m => m.CreateDate)
                .Skip(skipCount)
                .Take(takeCount);
        }
        public static IQueryable<T> UserPaginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
            where T : AppUser
        {
            var skipCount = ((pageNumber > 1 ? pageNumber : 1) - 1) * pageSize;
            var takeCount = pageSize;

            return query
                .OrderBy(m => m.DisplayName)
                .Skip(skipCount)
                .Take(takeCount);
        }
    }
}