using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Ses.AspNetCore.Framework.IRepository;
using Ses.AspNetCore.Framework.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ses.AspNetCore.Framework.Ioc
{
    public class AutofacContainer
    {
        // 容器构造器、容器、Assembly集合、Type集合、指定接口实例字典类集合 都是唯一共享的
        private static ContainerBuilder _containerBuilder = new ContainerBuilder();
        private static IContainer _container;
        private static string[] _assemblies;
        private static List<Type> _typeList = new List<Type>();
        private static Dictionary<Type, Type> _dictionary = new Dictionary<Type, Type>();
        private static Dictionary<Type, Type> _dictonaryGeneric = new Dictionary<Type, Type>();

        /// <summary>
        /// 注册程序集（集合）
        /// </summary>
        /// <param name="assemblies"></param>
        public static void Register(params string[] assemblies)
        {
            _assemblies = assemblies;
        }

        /// <summary>
        /// 注册Type（集合） 
        /// </summary>
        /// <param name="list"></param>
        public static void Register(params Type[] types)
        {
            _typeList.AddRange(types);
        }

        /// <summary>
        /// 注册Int-->Imp的字典类
        /// </summary>
        /// 
        public static void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _dictionary.Add(typeof(TInterface), typeof(TImplementation));
        }

        /// <summary>
        ///  注册Int-->Imp 对于程序集
        /// </summary>
        /// <param name="implementationAssemblyName">Imp程序集</param>
        /// <param name="interfaceAssemblyName">Int程序集</param>
        public static void Register(string implementationAssemblyName, string interfaceAssemblyName)
        {
            var impAssembly = Assembly.Load(implementationAssemblyName);
            var intAssembly = Assembly.Load(interfaceAssemblyName);

            var impTypes = impAssembly.DefinedTypes    // 获得impAssembly程序集中定义的所有类型集合
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType && !x.IsNested); // 类&&(!抽象类)&&(!泛型类)&&(!嵌套类)

            foreach (var type in impTypes)
            {
                // 接口: IName  实例：Name
                var intTypeName = interfaceAssemblyName + ".I" + type.Name;
                var intType = intAssembly.GetType(intTypeName);
                if (intType!=null&& intType.IsAssignableFrom(type)) // 类型(type) 是否继承与 接口 (intType)
                {
                    _dictionary.Add(intType, type);
                }
            }
        }

        /// <summary>
        /// 注册泛型 Int-->Imp  implement必须实现@interface 接口
        /// </summary>
        /// <param name="@interface"></param>
        /// <param name="implement"></param>
        public static void RegisterGeneric(Type @interface, Type implement)
        {
            _dictonaryGeneric.Add(@interface, implement);
        }

        /// <summary>
        /// 注册一个实体类（SingleInstance）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void Register<T>(T instance) where T : class
        {
            _containerBuilder.RegisterInstance(instance).SingleInstance();
        }

        /// <summary>
        /// 构建ioc容器，在register完成后调用，并创建容器
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceProvider Build(IServiceCollection service)
        {
            //临时注册 仓储
            //_containerBuilder.RegisterGeneric(typeof(BaseRepository<,>)).As(typeof(IRepository<,>));

            // Assembly集合不为空
            if (_assemblies != null)
            {
                foreach (var assmbly in _assemblies)
                {
                    _containerBuilder.RegisterAssemblyTypes(Assembly.Load(assmbly));
                }
            }

            // Types集合不为空
            if (_typeList != null)
            {
                foreach (var type in _typeList)
                {
                    _containerBuilder.RegisterType(type);
                }
            }

            // Dictionary集合不为空
            if (_dictionary != null)
            {
                foreach (var item in _dictionary)
                {
                    _containerBuilder.RegisterType(item.Value).As(item.Key);
                }
            }

            if (_dictonaryGeneric != null)
            {
                foreach (var item in _dictonaryGeneric)
                {
                    _containerBuilder.RegisterGeneric(item.Value).As(item.Key);
                }
            }

            // 将ServicesCollection 转移到 容器构造器中
            _containerBuilder.Populate(service);
            // 创建容器
            _container = _containerBuilder.Build();
            return new AutofacServiceProvider(_container);
        }


        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }


    }
}
