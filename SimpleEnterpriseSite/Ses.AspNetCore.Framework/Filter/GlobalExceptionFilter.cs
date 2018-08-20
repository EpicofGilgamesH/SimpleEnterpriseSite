using Microsoft.AspNetCore.Mvc.Filters;
using Ses.AspNetCore.Framework.Helper;
using System.Reflection;

namespace Ses.AspNetCore.Framework.Filter
{
    /// <summary>
    /// 全局异常捕获类
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            //获取当前异常抛出方法成员的类
            var type = MethodBase.GetCurrentMethod().DeclaringType;
            Log4NetHelper.WriteError(type, context.Exception);
        }
    }
}
