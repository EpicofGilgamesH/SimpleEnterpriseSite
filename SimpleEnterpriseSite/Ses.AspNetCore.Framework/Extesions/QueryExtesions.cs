using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ses.AspNetCore.Framework.Extesions
{
    public static class QueryExtesions
    {
        public static IQueryable<T> GetTableQueryable<T>
            (IQueryable<T> query,
            List<Expression<Func<T, bool>>> searchExpression,
            Dictionary<Expression<Func<T, object>>, bool> orderExpression,
            out int totalCount, bool isDesc = true)
        {
            if (searchExpression != null && searchExpression.Count > 0)
            {
                foreach (var func in searchExpression)
                {
                    query = query.Where(func);
                }
            }
            if (orderExpression != null && orderExpression.Count > 0)
            {
                foreach (var item in orderExpression)
                {
                    if (item.Value)
                        query.OrderByDescending(item.Key);
                    else
                        query.OrderBy(item.Key);
                }
            }
            totalCount = query.Count();
            return query;
        }

    }
}
