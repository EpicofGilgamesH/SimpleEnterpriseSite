using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using log4net.Repository;
using log4net;
using Ses.AspNetCore.Framework.Helper;
using Ses.AspNetCore.Framework.Filter;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Ses.AspNetCore.Framework.Option;
using Ses.AspNetCore.Framework.Ioc;
using Ses.AspNetCore.Framework;
using Ses.AspNetCore.Framework.IRepository;
using Ses.AspNetCore.Framework.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Ses.AspNetCore.Framework.AutoMapper;
using Ses.AspNetCore.Framework.IService;
using Ses.AspNetCore.Framework.Service;

namespace Ses.AspNetCore.Backstage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //初始化log4net 
            LoggerRepository = LogManager.CreateRepository("NETCoreRepository");
            Log4NetHelper.SetConfig(LoggerRepository, "log4net.config");
        }

        public static ILoggerRepository LoggerRepository { get; set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Mvc 添加全局异常捕获写入日志的Filter
            services.AddMvc(option => option.Filters.Add(new GlobalExceptionFilter()));
            services.AddAutoMapper();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/Account/Login"));

            // 启用MemoryCache
            services.AddMemoryCache();
            services.AddDistributedRedisCache(option =>
            {
                // redis连接字符串
                option.Configuration = "localhost";
                // Redis实例名称
                option.InstanceName = "";
            });
            // 启用Redis
            services.Configure<MemoryCacheEntryOptions>(
                    options => options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)) // 设置MemoryCache缓存有效时间为5分钟。
                .Configure<DistributedCacheEntryOptions>(option =>
                    option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5));// 设置Redis缓存有效时间为5分钟。

            // 使用Autofac ioc容器
            return InitIoc(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            // 初始化automapping
            //RegisterMapper.MapperRegister();

        }



        /// <summary>
        /// 初始化Ioc容器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private IServiceProvider InitIoc(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("MsSqlServer");
            //var connectionString = Configuration.GetConnectionString("MySql");
            var dbContextOption = new DbContextOption
            {
                ConnectionString = connectionString,
                DbType = DbTypeEnum.MSSQLSERVER,
                //DbType = DbTypeEnum.MYSQL,
                ModelAssemblyName = "Ses.AspNetCore.Entities",

            };
            //var codeGenerateOption = new CodeGenerateOption
            //{
            //    ModelsNamespace = "Zxw.Framework.Website.Models",
            //    IRepositoriesNamespace = "Zxw.Framework.Website.IRepositories",
            //    RepositoriesNamespace = "Zxw.Framework.Website.Repositories",
            //    IServicsNamespace = "Zxw.Framework.Website.IServices",
            //    ServicesNamespace = "Zxw.Framework.Website.Services"
            //};
            AutofacContainer.Register(Configuration);//注册配置
            AutofacContainer.Register(dbContextOption);//注册数据库配置信息
            //AutofacContainer.Register(codeGenerateOption);//注册代码生成器相关配置信息
            AutofacContainer.Register(typeof(DefaultDbContext));//注册EF上下文
            AutofacContainer.RegisterGeneric(typeof(IRepository<,>), typeof(BaseRepository<,>));
            AutofacContainer.RegisterGeneric(typeof(IBaseService<,>), typeof(BaseService<,>));
            AutofacContainer.Register("Ses.AspNetCore.Services", "Ses.AspNetCore.IServices");//注册service

            return AutofacContainer.Build(services);

        }
    }
}
