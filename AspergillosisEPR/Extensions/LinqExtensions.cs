using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static partial class LinqExtensions
    {
        public static IQueryable Query(this DbContext context, string entityName) =>
            context.Query(context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));

        public static IQueryable Query(this DbContext context, Type entityType) =>
            (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);

        public static IEnumerable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, 
                                                                                       IEnumerable<TRight> right, 
                                                                                       Func<TLeft, TKey> leftKey, 
                                                                                       Func<TRight, TKey> rightKey,
                                                                                       Func<TLeft, TRight, TResult> result)
        {
            return left.GroupJoin(right, leftKey, rightKey, (l, r) => new { l, r })
                 .SelectMany(
                     o => o.r.DefaultIfEmpty(),
                     (l, r) => new { lft = l.l, rght = r })
                 .Select(o => result.Invoke(o.lft, o.rght));
        }

        /* 
         * 
         * Linq left outer join example
         * var contents = list.LeftOuterJoin(list2, 
             l => l.country, 
             r => r.name,
            (l, r) => new { count = l.Count(), l.country, l.reason, r.people })
            */
    }
}
 