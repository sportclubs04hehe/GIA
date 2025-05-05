using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Helpers
{
    public static class QueryableExtensions
    {
      public static IQueryable<T> WhereIlikeAny<T>(
        this IQueryable<T> source,
        string term,
        params Expression<Func<T, string>>[] selectors
      )
        {
            if (string.IsNullOrWhiteSpace(term) || selectors == null || selectors.Length == 0)
                return source;

            var pattern = $"%{term.Trim()}%";
            Expression<Func<T, bool>> predicate = x => false;

            foreach (var sel in selectors)
            {
                // EF.Functions.ILike(sel(x), pattern)
                var method = typeof(NpgsqlDbFunctionsExtensions)
                    .GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
                        new[] { typeof(DbFunctions), typeof(string), typeof(string) })!;

                var call = Expression.Call(
                    method,
                    Expression.Constant(EF.Functions),
                    Expression.Invoke(sel, sel.Parameters[0]),
                    Expression.Constant(pattern)
                );

                var lambda = Expression.Lambda<Func<T, bool>>(call, sel.Parameters);
                predicate = predicate.Or(lambda);
            }

            return source.Where(predicate);
        }

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

        private static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var invoked = Expression.Invoke(right, left.Parameters);
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(left.Body, invoked), left.Parameters);
        }
    }
}
