using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ses.AspNetCore.Framework.IRepository
{
    /// <summary>
    /// 仓储 泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<T, TKey> : IDisposable
        where T : class, IEntityBase<TKey>
    {
        IQueryable<T> Entities { get; }
        IQueryable<T> EntitiesNoTracking { get; }
        int Add(T entity);
        int AddRange(ICollection<T> entities);
        int Count(Expression<Func<T, bool>> @where = null);
        int Delete(TKey key);
        int Delete(T entity);
        int Delete(Expression<Func<T, bool>> @where);
        int Edit(T entity);
        int EditRange(ICollection<T> entities);
        bool Exist(Expression<Func<T, bool>> @where = null);
        bool Exist(Expression<Func<T, bool>> @where = null, params Expression<Func<T, object>>[] includes);
        int ExecuteSqlWithNonQuery(string sql, params object[] parameters);
        T GetSingle(TKey key);
        T GetSingle(TKey key, params Expression<Func<T, object>>[] includes);
        T GetSingle(Expression<Func<T, bool>> @where = null);
        T GetSingle(Expression<Func<T, bool>> @where = null, params Expression<Func<T, object>>[] includes);
        IQueryable<T> Get(Expression<Func<T, bool>> @where = null);
        IQueryable<T> Get(Expression<Func<T, bool>> @where = null, params Expression<Func<T, object>>[] includes);
        //IEnumerable<T> GetByPagination(Expression<Func<T, bool>> @where, int pageSize, int pageIndex, bool asc = true,
        //    params Func<T, object>[] @orderby);
        IList<T> GetByPagination(Expression<Func<T, bool>> @where, int pageSize, int pageIndex, Dictionary<Expression<Func<T, object>>, bool> OrderDictionary, out int totalCount);
        IList<TView> SqlQuery<TView>(string sql, params object[] parameters) where TView : class, new();
        bool Transcation(Action<DbContext> action);
        void BulkInsert(IList<T> entities, string destinationTableName = null);
        IList<TModel> RawSqlQuery<TModel>(string sql, Func<DbDataReader, TModel> map, params object[] parameters);
    }
}
