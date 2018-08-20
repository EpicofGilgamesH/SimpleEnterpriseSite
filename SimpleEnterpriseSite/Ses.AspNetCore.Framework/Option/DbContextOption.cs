using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Option
{
    public class DbContextOption
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 实体程序集名称
        /// </summary>
        public string ModelAssemblyName { get; set; }
        /// <summary>
        /// 数据库类型 默认为MSSQLSERVER
        /// </summary>
        public DbTypeEnum DbType { get; set; } = DbTypeEnum.MSSQLSERVER;
    }

    public enum DbTypeEnum
    {
        /// <summary>
        /// MS SQL Server
        /// </summary>
        MSSQLSERVER = 0,
        /// <summary>
        /// Oracle
        /// </summary>
        ORACLE,
        /// <summary>
        /// MySQL
        /// </summary>
        MYSQL,
        /// <summary>
        /// Sqlite
        /// </summary>
        SQLITE,
        /// <summary>
        /// in-memory database
        /// </summary>
        MEMORY,
        /// <summary>
        /// PostgreSQL
        /// </summary>
        NPGSQL
    }
}
