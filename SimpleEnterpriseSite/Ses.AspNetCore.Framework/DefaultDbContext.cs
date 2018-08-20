using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities;
using Ses.AspNetCore.Framework.Extesions;
using Ses.AspNetCore.Framework.Option;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Ses.AspNetCore.Framework
{
    public class DefaultDbContext : DbContext
    {
        private const string mappingAssmbly = "Ses.AspNetCore.Framework";
        private DbContextOption _option;
        public DefaultDbContext(DbContextOption option)
        {
            // option 的ConnectionString属性、ModelAssemblyName属性都不能为空
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            if (string.IsNullOrEmpty(option.ConnectionString))
                throw new ArgumentNullException(nameof(option.ConnectionString));
            if (string.IsNullOrEmpty(option.ModelAssemblyName))
                throw new ArgumentNullException(nameof(option.ModelAssemblyName));
            _option = option;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_option.DbType)
            {
                case DbTypeEnum.ORACLE:
                    throw new NotSupportedException("Oracle EF Core Database Provider is not yet available.");
                case DbTypeEnum.MYSQL:
                    optionsBuilder.UseMySql(_option.ConnectionString);
                    break;
                case DbTypeEnum.SQLITE:
                    optionsBuilder.UseSqlite(_option.ConnectionString);
                    break;
                case DbTypeEnum.MEMORY:
                    optionsBuilder.UseInMemoryDatabase(_option.ConnectionString);
                    break;
                case DbTypeEnum.NPGSQL:
                    optionsBuilder.UseNpgsql(_option.ConnectionString);
                    break;
                default:
                    optionsBuilder.UseSqlServer(_option.ConnectionString);
                    break;
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddEntityTypes(modelBuilder);
            // 将所有的map注册到ModelBuilder中
            var types = Assembly.Load(mappingAssmbly).GetTypes();
            foreach (var type in types)
            {
                if (Array.Exists(type.GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
                {
                    dynamic configurationInstance = Activator.CreateInstance(type);
                    modelBuilder.ApplyConfiguration(configurationInstance);
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        private void AddEntityTypes(ModelBuilder modelBuilder)
        {
            // 将所有继承IEntityBase 的实体类（Ses.AspNetCore.Entities程序集）
            var assembly = Assembly.Load(_option.ModelAssemblyName);
            var types = assembly?.GetTypes();
            var list = types?.Where(t =>
                t.IsClass && !t.IsGenericType && !t.IsAbstract &&
                t.GetInterfaces().Any(m => m.GetGenericTypeDefinition() == typeof(IEntityBase<>))).ToList();
            if (list != null && list.Any())
            {
                list.ForEach(t =>
                {
                    if (modelBuilder.Model.FindEntityType(t) == null)
                        modelBuilder.Model.AddEntityType(t);
                });
            }
        }

        #region EF数据操作方法
        /*
   
        /// <summary>
        /// ExecuteSqlWithNonQuery
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteSqlWithNonQuery(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sql,
                CancellationToken.None,
                parameters);
        }

        /// <summary>
        /// edit an entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Edit<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        /// <summary>
        /// edit entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int EditRange<T>(ICollection<T> entities) where T : class
        {
            Set<T>().AttachRange(entities.ToArray());
            return SaveChanges();
        }

        /// <summary>
        /// update query datas by columns.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="updateExp"></param>
        /// <returns></returns>
        //public int Update<T>(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp)
        //    where T : class
        //{
        //    return Set<T>().Where(@where).Update(updateExp);
        //}

        /// <summary>
        /// update data by columns.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="updateColumns"></param>
        /// <returns></returns>
        public int Update<T>(T model, params string[] updateColumns) where T : class
        {
            if (updateColumns != null && updateColumns.Length > 0)
            {
                if (Entry(model).State == EntityState.Added ||
                    Entry(model).State == EntityState.Detached) Set<T>().Attach(model);
                foreach (var propertyName in updateColumns)
                {
                    Entry(model).Property(propertyName).IsModified = true;
                }
            }
            else
            {
                Entry(model).State = EntityState.Modified;
            }
            return SaveChanges();
        }

        /// <summary>
        /// delete by query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<T>(Expression<Func<T, bool>> @where) where T : class
        {
            var entities = Set<T>().Where(@where);
            foreach (var entity in entities)
            {
                Entry(entity).State = EntityState.Deleted;
            }

            return SaveChanges();
        }

        public List<TView> SqlQuery<T, TView>(string sql, params object[] parameters)
                where T : class
                where TView : class
        {
            return Set<T>().FromSql(sql, parameters).Cast<TView>().ToList();
        }

    */
        #endregion

        /// <summary>
        /// bulk insert by sqlbulkcopy, and with transaction.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="destinationTableName"></param>
        public void BulkInsert<T>(IList<T> entities, string destinationTableName = null) where T : class
        {
            if (entities == null || !entities.Any()) return;
            if (string.IsNullOrEmpty(destinationTableName))
            {
                var mappingTableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                destinationTableName = string.IsNullOrEmpty(mappingTableName) ? typeof(T).Name : mappingTableName;
            }
            using (var dt = entities.ToDataTable())
            {
                using (var conn = new SqlConnection(_option.ConnectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                            bulk.BatchSize = entities.Count;
                            bulk.DestinationTableName = destinationTableName;
                            bulk.EnableStreaming = true;
                            bulk.WriteToServerAsync(dt);
                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                    conn.Close();
                }
            }
        }

    }
}
