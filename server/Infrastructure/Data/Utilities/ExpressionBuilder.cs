// 1. Tạo class xử lý biểu thức tìm kiếm
using Core.Entities.IdentityBase;
using System.Linq.Expressions;

namespace Infrastructure.Data.Utilities
{
    public class ExpressionBuilder<T> where T : class
    {
        public IQueryable<T> ApplySearch(
            IQueryable<T> query,
            string searchTerm,
            Expression<Func<T, string>>[] searchFields)
        {
            searchTerm = searchTerm.ToLower();
            Expression<Func<T, bool>> combinedExpression = null;

            foreach (var selector in searchFields)
            {
                var memberExpression = selector.Body as MemberExpression;
                if (memberExpression == null) continue;

                string propertyName = memberExpression.Member.Name;
                var parameter = Expression.Parameter(typeof(T), "x");

                var property = Expression.Property(parameter, propertyName);
                var toLower = Expression.Call(property,
                    typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                var contains = Expression.Call(toLower,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                    Expression.Constant(searchTerm));

                var expression = Expression.Lambda<Func<T, bool>>(contains, parameter);

                combinedExpression = combinedExpression == null
                    ? expression
                    : CombineOr(combinedExpression, expression);
            }

            return combinedExpression != null ? query.Where(combinedExpression) : query;
        }

        public Expression<Func<T, bool>> CombineOr(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var visitor = new ParameterReplacer(expr2.Parameters[0], parameter);
            var body2Modified = visitor.Visit(expr2.Body);
            visitor = new ParameterReplacer(expr1.Parameters[0], parameter);
            var body1Modified = visitor.Visit(expr1.Body);
            var combined = Expression.OrElse(body1Modified, body2Modified);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}