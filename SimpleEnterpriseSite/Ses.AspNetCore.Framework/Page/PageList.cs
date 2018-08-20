using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Linq
{
    public static class PageList
    {
        public static IQueryable<T> GetPageList<T>(this IQueryable<T> queryable,
            int pagesize, int pageindex)
        {
            return queryable.Skip(pageindex * pagesize).Take(pagesize);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">获取条件查询、多排序 的IQueryable</typeparam>
        /// <param name="queryable">集合 延迟查询</param>
        /// <param name="searchExpression">查询条件表达式</param>
        /// <param name="orderDic">排序[字段-是否倒序]字典集合</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static IQueryable<T> GetTableQueryable<T>
                (this IQueryable<T> queryable,
                List<Expression<Func<T, bool>>> searchExpression,
                Dictionary<string, bool> orderDic, out int totalCount)
        {
            queryable = queryable.ConditionQueryable(searchExpression, out totalCount);

            queryable = queryable.GetOrderQueryable(orderDic);


            #region 这种方式达不到目的，Func<T,object> 在装载 实例lambda x=>x.UpdateTime 时，会进行转换  变成 key={x=>Convert(x.UpdateTime,Object)} 此时，Linq解析成SQL时会与预期效果不一致 "ORDER BY (SELECT 1)"
            //if (orderExpression != null && orderExpression.Count > 0)
            //{
            //    foreach (var item in orderExpression)
            //    {
            //        if (item.Value)
            //        {
            //            queryable.OrderByDescending(item.Key);
            //        }
            //        else
            //        {
            //            queryable.OrderBy(item.Key);
            //        }
            //    }
            //}
            #endregion
            return queryable;
        }

        /// <summary>
        ///  获取条件查询、单排序 的IQueryable<T>
        /// </summary>
        /// <typeparam name="T">操作实体类型</typeparam>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="queryable">集合 延迟查询</param>
        /// <param name="searchExpression">查询条件表达式</param>
        /// <param name="orderExpression">排序表达式</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        public static IQueryable<T> GetTableQueryable<T, TKey>
        (this IQueryable<T> queryable,
        List<Expression<Func<T, bool>>> searchExpression,
        Expression<Func<T, TKey>> orderExpression,
        bool isDesc, out int totalCount)
        {
            queryable = queryable.ConditionQueryable(searchExpression, out totalCount);
            if (orderExpression != null)
            {
                if (isDesc)
                    queryable = queryable.OrderByDescending(orderExpression);
                else
                    queryable = queryable.OrderBy(orderExpression);
            }
            return queryable;
        }


        /// <summary>
        /// 排序查询
        /// </summary>
        /// <typeparam name="T">操作实体类型</typeparam>
        /// <param name="queryable">集合 延迟查询</param>
        /// <param name="dictionary">排序 字段-正反顺 集合</param>
        /// <returns></returns>
        public static IQueryable<T> GetOrderQueryable<T>
            (this IQueryable<T> queryable,
               Dictionary<string, bool> dictionary)
        {
            if (dictionary != null && dictionary.Count > 0)
            {
                var flag = false;
                foreach (var item in dictionary)
                {
                    if (!string.IsNullOrEmpty(item.Key))
                    {
                        try
                        {
                            var property = typeof(T).GetProperty(item.Key);
                            var tkey = property.PropertyType;
                            var method = typeof(PageList).GetMethod("GetOrdersQueryable",
                                BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeof(T), tkey);
                            //反射调用方法时，该方法为静态，则obj参数为null，该方法为实例方法，则obj为该方法的实例
                            queryable = (IQueryable<T>)method.Invoke(null, new object[] { queryable, item.Key, item.Value, flag });
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            return queryable;
        }


        #region private
        /// <summary>
        /// 通过排序属性名，调用Linq中的OrderByDescending等方法
        /// </summary>
        /// <typeparam name="T">操作实体类型</typeparam>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="queryable">集合 延迟查询</param>
        /// <param name="proportyName">排序字段名</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="flag">是否ThenBy</param>
        /// <returns></returns>
        private static IQueryable<T> GetOrdersQueryable<T, TKey>(this IQueryable<T> queryable, string proportyName, bool isDesc, bool flag)
        {
            var property = typeof(T).GetProperty(proportyName);
            if (property.PropertyType != typeof(TKey))
                throw new Exception("该排序属性不存在");
            var arg = Expression.Parameter(typeof(T));
            var lambda = Expression.Lambda<Func<T, TKey>>(Expression.Property(arg, property), arg);

            if (isDesc) //desc
            {
                if (flag)
                {
                    IOrderedQueryable<T> queryableOrder = queryable as IOrderedQueryable<T>;
                    queryable = queryableOrder.ThenByDescending(lambda);
                }
                else
                {
                    queryable = queryable.OrderByDescending(lambda);
                }
            }
            else  //asc
            {
                if (flag)
                {
                    IOrderedQueryable<T> queryableOrder = queryable as IOrderedQueryable<T>;
                    queryable = queryableOrder.ThenBy(lambda);
                }
                else
                {
                    queryable = queryable.OrderBy(lambda);
                }
            }
            return queryable;
        }


        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="T">操作实体类型</typeparam>
        /// <param name="queryable">集合 延迟查询</param>
        /// <param name="searchExpression">查询条件表达式</param>
        /// <returns>集合</returns>
        private static IQueryable<T> ConditionQueryable<T>(this IQueryable<T> queryable,
            List<Expression<Func<T, bool>>> searchExpression, out int totalCount)
        {
            if (searchExpression != null && searchExpression.Count > 0)
            {
                foreach (var func in searchExpression)
                {
                    queryable = queryable.Where(func);
                }
            }
            totalCount = queryable.Count();
            return queryable;
        }

        #endregion
    }
}

