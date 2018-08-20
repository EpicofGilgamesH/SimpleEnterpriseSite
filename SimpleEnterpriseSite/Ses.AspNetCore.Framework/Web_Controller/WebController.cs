using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Ses.AspNetCore.Framework.Base_Controller;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Ses.AspNetCore.Framework.Extesions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ses.AspNetCore.Framework.Web_Controller
{
    public abstract class WebController : BaseController
    {
        private LoginInfoSession _loginInfoSession;

        public LoginInfoSession UserInfoSession
        {
            // get 读（获取属性值）:当前_loginInfoSession不为空则将其读入属性
            //           当前身份认证为通过，则属性读入null
            get
            {
                if (_loginInfoSession != null)
                    return _loginInfoSession;
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return null;
                var userSession = LoginInfoSession.GetLoginInfoSeesionByPrincipal(HttpContext.User);
                _loginInfoSession = userSession;
                return _loginInfoSession;
            }
            // set 写（为属性赋值）：当写入的用户登陆信息为null时，注销登陆
            //                                   当写入的用户登陆信息正确时，登陆
            set
            {
                var UserSeesion = value;
                if (UserSeesion == null) //注销登陆
                {
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return;
                }

                List<Claim> claims = UserSeesion.GetClaims();
                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                //登陆
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
                    IsPersistent = false,
                    AllowRefresh = false
                });

                _loginInfoSession = UserSeesion;
            }
        }


        /// <summary>
        /// 进入控制器前账户进行权限认证,已经登出的账户，访问页面时跳转到登陆页面
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            HttpRequest httpRequest = filterContext.HttpContext.Request;
            var controllerName = filterContext.RouteData.Values["Controller"].ToString();
            var actionName = filterContext.RouteData.Values["Action"].ToString();
            var isLoginPath = controllerName == "Account" && actionName == "Login";
            //如果已登陆 或 是登陆页面则跳过
            if (filterContext.HttpContext.User.Identity.IsAuthenticated || isLoginPath)
            {
                return;
            }

            //Http请求分为Ajax请求和普通请求
            if (httpRequest.isAjaxRequest())
            {
                SesJsonResult jsonResult = new SesJsonResult(JsonResultStatus.NotLogin, "未登陆或登陆超时，请重新登陆");
                ContentResult contentResult = new ContentResult() { Content = JsonConvert.SerializeObject(jsonResult) };
                filterContext.Result = contentResult;
                return;
            }
            else
            {
                var url = "/Account/Login";
                url = string.Concat(url, "?returnUrl=", httpRequest.Path);
                //跳转页面
                //RedirectResult redirectResult = new RedirectResult(url);
                //filterContext.Result = redirectResult;

                HttpContext.Response.WriteAsync("<script>window.parent.location='" + url + "'</script>");
                //filterContext.HttpContext.Response.WriteAsync("<script>window.parent.location.href=" + url + "</script>");
                return;
            }
        }



    }
}
