using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Ioc
{
    public class AppServiceFactory : IAppServiceFactory
    {
        IServiceProvider _serviceProvider = null;

        public AppServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual T CreateService<T>() where T : class
        {
            object serviceObj = this._serviceProvider.GetService(typeof(T));
            if (serviceObj == null)
                throw new Exception("Can not find the service.");
            T service = serviceObj as T;
            return service;
        }

        public void Dispose()
        {

        }
    }
}
