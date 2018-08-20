using Ses.AspNetCore.Entities;
using Ses.AspNetCore.Framework.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Ses.AspNetCore.Framework.Extesions;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace Ses.AspNetCore.Framework.Repository
{
    /// <summary>
    /// 抽象仓储基础类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class BaseRepository<T, TKey> : IRepository<T, TKey>
       where T : class, IEntityBase<TKey>
    {
        private readonly DefaultDbContext _dbContext;
        private readonly DbSet<T> _dbset;

        public BaseRepository(DefaultDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            //确保数据库已经被创建
            _dbContext.Database.EnsureCreated();
            _dbset = dbContext.Set<T>();
        }

        public IQueryable<T> Entities => _dbset;

        public IQueryable<T> EntitiesNoTracking => _dbset.AsNoTracking();

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(T entity)
        {
            int i = -1;
            _dbset.Add(entity);
            i = _dbContext.SaveChanges();
            return i;
        }

        /// <summary>
        /// 批量新增实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int AddRange(ICollection<T> entities)
        {
            int i = -1;
            _dbset.AddRange(entities);
            i = _dbContext.SaveChanges();
            return i;
        }

        public void BulkInsert(IList<T> entities, string destinationTableName = null)
        {
            _dbContext.BulkInsert(entities, destinationTableName);
        }

        /// <summary>
        /// 查询条数
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> where = null)
        {
            return where == null ? _dbset.Count() : _dbset.Count(where);
        }

        /// <summary>
        /// 通过主键删除数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Delete(TKey key)
        {
            int i = -1;
            var entity = _dbset.Find(key);
            if (entity == null)
                return 0;
            _dbset.Remove(entity);
            i = _dbContext.SaveChanges();
            return i;
        }

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(T entity)
        {
            int i = -1;
            if (entity == null)
                return 0;
            _dbset.Remove(entity);
            i = _dbContext.SaveChanges();
            return i;
        }

        /// <summary>
        /// 通过条件删除
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete(Expression<Func<T, bool>> where)
        {
            int i = -1;
            var entities = _dbset.Where(where);
            _dbset.RemoveRange(entities);
            i = _dbContext.SaveChanges();
            return i;
        }

        /// <summary>
        /// 通过单个实体更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Edit(T entity)
        {
            int i = -1;
            _dbset.Update(entity);
            i = _dbContext.SaveChanges();
            return i;
        }

        /// <summary>
        /// 通过实体集合更新
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int EditRange(ICollection<T> entities)
        {
            int i = -1;
            _dbset.UpdateRange(entities);
            i = _dbContext.SaveChanges();
            return i;
        }

        /// <summary>
        /// 执行ExecuteSqlWhithNonQuery
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteSqlWithNonQuery(string sql, params object[] parameters)
        {
            int i = -1;
            try
            {
                i = _dbContext.Database.ExecuteSqlCommand(sql, CancellationToken.None, parameters);
            }
            catch (Exception ex)
            {

            }
            return i;
        }

        public bool Exist(Expression<Func<T, bool>> where = null)
        {
            return (where == null ? EntitiesNoTracking.Count()
                : EntitiesNoTracking.Where(where).Count()) > 0;
        }

        public bool Exist(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> where = null)
        {
            return where != null ? EntitiesNoTracking.Where(where) : EntitiesNoTracking;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IList<T> GetByPagination(Expression<Func<T, bool>> where, int pageSize,
            int pageIndex, Dictionary<Expression<Func<T, object>>, bool> OrderDictionary, out int totalCount)
        {
            var filter = Get(where);
            if (OrderDictionary != null && OrderDictionary.Count > 0)
            {
                foreach (var item in OrderDictionary)
                {
                    // asc or desc
                    filter = item.Value ? filter.OrderBy(item.Key) : filter.OrderByDescending(item.Key);
                }
            }
            totalCount = filter.Count();
            // 从第一页开始
            return filter.Skip(pageSize * pageIndex - 1).Take(pageSize).ToList();
        }


        public T GetSingle(TKey key)
        {
            return _dbset.Find(key);
        }

        public T GetSingle(TKey key, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null) return GetSingle(key);
            var query = _dbset.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            Expression<Func<T, bool>> filter = m => m.Id.Equal(key);
            return query.SingleOrDefault(filter.Compile());
        }

        public T GetSingle(Expression<Func<T, bool>> where = null)
        {
            if (where == null) return _dbset.SingleOrDefault();
            return _dbset.SingleOrDefault(@where);
        }

        public T GetSingle(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null) return GetSingle(where);
            var query = _dbset.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            if (where == null) return query.SingleOrDefault();
            return query.SingleOrDefault(where);
        }

        /// <summary>
        /// 执行sql查询，返回集合
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<TView> SqlQuery<TView>(string sql, params object[] parameters) where TView : class, new()
        {
            return _dbContext.Set<T>().FromSql(sql, parameters).Cast<TView>().ToList();
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Transcation(Action<DbContext> action)
        {
            var flag = false;
            using (var tanscation = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    action(_dbContext);
                    tanscation.Commit();
                    flag = true;
                }
                catch (Exception ex)
                {
                    tanscation.Rollback();
                }
            }
            return flag;
        }

        public IList<TModel> RawSqlQuery<TModel>(string sql, Func<DbDataReader, TModel> map, params object[] parameters)
        {
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                //同一个Parameters 会进行引用地址检查，不能将相同的引用地址的Parameters用到多个sql语句中
                command.Parameters.AddRange(parameters);
                //command.Parameters.AddRange(parameters.Select(x => ((ICloneable)x).Clone()).ToArray());
                _dbContext.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<TModel>();
                    while (result.Read())
                    {
                        entities.Add(map(result));
                    }
                    return entities;
                }
            }
        }



        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }


    }
}
