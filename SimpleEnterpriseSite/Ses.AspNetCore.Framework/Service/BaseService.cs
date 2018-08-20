using Ses.AspNetCore.Entities;
using Ses.AspNetCore.Framework.IService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Ses.AspNetCore.Framework.IRepository;
using System.Linq;

namespace Ses.AspNetCore.Framework.Service
{
    /// <summary>
    /// 服务类基础抽象类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class BaseService<T, TKey> : IBaseService<T, TKey>
         where T : class, IEntityBase<TKey>
    {
        protected IRepository<T, TKey> _repository;

        public BaseService(IRepository<T, TKey> repository)
        {
            _repository = repository;
        }
        public int Add(T entity)
        {
            return _repository.Add(entity);
        }

        public int AddRange(ICollection<T> entities)
        {
            return _repository.AddRange(entities);
        }

        public int Count(Expression<Func<T, bool>> where = null)
        {
            return _repository.Count(where);
        }

        public IList<TView> CustomQuery<TView>(string sql, params object[] parameters) where TView : class, new()
        {
            return _repository.SqlQuery<TView>(sql, parameters);
        }

        public int Delete(TKey key)
        {
            return _repository.Delete(key);
        }

        public int Delete(Expression<Func<T, bool>> where)
        {
            return _repository.Delete(where);
        }

        public int Edit(T entity)
        {
            return _repository.Edit(entity);
        }

        public int EditRange(ICollection<T> entities)
        {
            return _repository.EditRange(entities);
        }

        public int ExecuteSqlWithNonQuery(string sql, params object[] parameters)
        {
            return _repository.ExecuteSqlWithNonQuery(sql, parameters);
        }

        public bool Exists(Expression<Func<T, bool>> where = null)
        {
            return _repository.Exist(where);
        }

        public bool Exists(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> where = null)
        {
            return _repository.Get(where);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public T GetSingle(TKey key)
        {
            return _repository.GetSingle(key);
        }

        public T GetSingle(TKey key, params Expression<Func<T, object>>[] includes)
        {
            return _repository.GetSingle(key, includes);
        }

        /*
        public int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> updateExp)
        {
            throw new NotImplementedException();
        }

        public int Update(T model, params string[] updateColumns)
        {
            throw new NotImplementedException();
        }

       */
        public void Dispose()
        {
            _repository.Dispose();
        }

        public IList<T> GetPageList(Expression<Func<T, bool>> where
            , int pageSize, int pageIndex
            , Dictionary<Expression<Func<T, object>>, bool> OrderDictionary, out int totalCount)
        {
            return _repository.GetByPagination(where, pageSize, pageIndex, OrderDictionary, out totalCount);
        }
    }
}
