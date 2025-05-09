using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Helpers
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderByProperty<T>(
            this IQueryable<T> source,
            string propertyName,
            bool descending = false)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property name phải khác null hoặc empty", nameof(propertyName));

            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.PropertyOrField(param, propertyName);
            var lambda = Expression.Lambda(prop, param);
            var method = descending ? "OrderByDescending" : "OrderBy";

            var call = Expression.Call(
                typeof(Queryable),
                method,
                new[] { typeof(T), prop.Type },
                source.Expression,
                Expression.Quote(lambda)
            );

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }
    }
}
