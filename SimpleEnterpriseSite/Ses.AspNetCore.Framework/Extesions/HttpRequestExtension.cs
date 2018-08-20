using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.Extesions
{
    public static class HttpRequestExtension
    {
        public static bool isAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request is null.");
            }
            return request.Headers.ContainsKey("X-Requested-With") && request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
