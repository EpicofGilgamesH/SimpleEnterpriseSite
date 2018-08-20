using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Ioc
{
    public interface IAppServiceFactory : IDisposable
    {
        T CreateService<T>() where T : class;
    }
}
