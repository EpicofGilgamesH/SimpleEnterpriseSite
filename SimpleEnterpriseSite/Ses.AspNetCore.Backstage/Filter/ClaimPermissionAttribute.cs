using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Ses.AspNetCore.Backstage.Controllers;
using Ses.AspNetCore.Backstage.Permission;
using Ses.AspNetCore.Framework.Base_Controller;
using Ses.AspNetCore.Framework.Ioc;
using Ses.AspNetCore.Framework.Web_Controller;
using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ses.AspNetCore.Backstage.Filter
{
    /// <summary>
    /// 页面权限控制过滤器
    /// </summary>
    public class ClaimPermissionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            WebController defaultController = (WebController)context.Controller;
            if (defaultController.UserInfoSession == null) return;
            AppServiceFactory appServiceFactory = new AppServiceFactory(context.HttpContext.RequestServices);
            var permissionService = appServiceFactory.CreateService<IPermissionService>();

            var claims = permissionService.GetUnionPermission(defaultController.UserInfoSession);
            //页面无权限过滤
            var url = $"/{context.RouteData.Values["Controller"].ToString()}/{context.RouteData.Values["Action"].ToString()}";
            var flag = claims.Where(x => x.Url != null).Where(x => x.Url.ToUpper() == url.ToUpper()).Count() > 0;
            if (!flag)
            {
                SesJsonResult jsonResult = new SesJsonResult(JsonResultStatus.Unauthorized, "无权限");
                context.Result = new ContentResult() { Content = JsonConvert.SerializeObject(jsonResult) };
                return;
            }

            //按钮权限控制
            var btnPermission = permissionService.GetUnionBtnPermission(defaultController.UserInfoSession, url);
            Dictionary<string, string> btnPermissionDic = new Dictionary<string, string>();
            var style = "display:none;";
            foreach (var item in BtnPermission.AllBtnPms.Split(','))
            {
                if (!btnPermission.Contains(item))
                    btnPermissionDic.Add(item, style);
                else
                    btnPermissionDic.Add(item, string.Empty);
            }
            defaultController.ViewBag.BtnPermissionDic = btnPermissionDic;
            base.OnActionExecuting(context);
        }

    }
}
