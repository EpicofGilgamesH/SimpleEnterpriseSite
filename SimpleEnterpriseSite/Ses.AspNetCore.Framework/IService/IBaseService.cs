using Ses.AspNetCore.Entities;
using Ses.AspNetCore.Framework.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ses.AspNetCore.Framework.IService
{
    public interface IBaseService<T, in TKey> : IDisposable
      where T : class, IEntityBase<TKey>
    {
        int Count(Expression<Func<T, bool>> where = null);

        bool Exists(Expression<Func<T, bool>> where = null);

        bool Exists(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes);

        T GetSingle(TKey key);

        T GetSingle(TKey key, params Expression<Func<T, object>>[] includes);

        IQueryable<T> Get(Expression<Func<T, bool>> where = null);

        IQueryable<T> Get(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes);

        IList<T> GetPageList(Expression<Func<T, bool>> where, int pageSize, int pageIndex, Dictionary<Expression<Func<T, object>>, bool> OrderDictionary, out int totalCount);

        int Add(T entity);

        int AddRange(ICollection<T> entities);

        int Edit(T entity);

        int EditRange(ICollection<T> entities);

        /*
        int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> updateExp);

        int Update(T model, params string[] updateColumns);

        */

        int Delete(TKey key);

        int Delete(Expression<Func<T, bool>> where);

        int ExecuteSqlWithNonQuery(string sql, params object[] parameters);

        IList<TView> CustomQuery<TView>(string sql, params object[] parameters) where TView : class, new();

    }
}
