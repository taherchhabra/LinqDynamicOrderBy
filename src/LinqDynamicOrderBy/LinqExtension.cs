using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqDynamicOrderBy
{
    public static class LinqExtension
    {
        public static IQueryable<TEntity> DynamicOrderBy<TEntity>(this IQueryable<TEntity> query, string orderByColumnsWithDirection)
        {

            var columnWithDirectionList = orderByColumnsWithDirection.Split(',');

            Expression orderByCallExpression = query.Expression;
            bool firstColumnFlag = true;
            foreach (var column in columnWithDirectionList)
            {

                var columnInfo = column.Split(' ');
                var columnName = columnInfo[0];
                var columnDirection = columnInfo[1];


                var type = typeof(TEntity);
                var property = type.GetProperty(columnName);
                var parameter = Expression.Parameter(type, "prop");

                if (firstColumnFlag)
                {
                    firstColumnFlag = false;
                    switch (columnDirection.ToLower())
                    {

                        case "desc":
                            orderByCallExpression = Expression.Call(
                                typeof(Queryable),
                                "OrderByDescending",
                                new Type[] { type, property.PropertyType },
                                orderByCallExpression,
                                Expression.Quote(Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter)));
                            break;
                        default:
                            orderByCallExpression = Expression.Call(
                                typeof(Queryable),
                                "OrderBy",
                                new Type[] { type, property.PropertyType },
                                orderByCallExpression,
                                Expression.Quote(Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter)));
                            break;
                    }
                }
                else
                {
                    switch (columnDirection.ToLower())
                    {

                        case "desc":
                            orderByCallExpression = Expression.Call(
                                typeof(Queryable),
                                "ThenByDescending",
                                new Type[] { type, property.PropertyType },
                                orderByCallExpression,
                                Expression.Quote(Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter)));
                            break;
                        default:
                            orderByCallExpression = Expression.Call(
                                typeof(Queryable),
                                "ThenBy",
                                new Type[] { type, property.PropertyType },
                                orderByCallExpression,
                                Expression.Quote(Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter)));
                            break;
                    }
                }
            }

            return query.Provider.CreateQuery<TEntity>(orderByCallExpression);

        }
    }
}
