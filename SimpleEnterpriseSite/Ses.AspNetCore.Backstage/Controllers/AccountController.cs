using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ses.AspNetCore.IServices;
using System.Security.Claims;
using Ses.AspNetCore.Framework;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Helper;
using Ses.AspNetCore.Framework.Web_Controller;

namespace Ses.AspNetCore.Backstage.Controllers
{

    public class AccountController : WebController
    {
        private IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 账号登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password">加密后的密码</param>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return base.FailedMsg("用户名/密码不能为空");

            var msg = string.Empty;
            if (!_accountService.SingIn(userName.Trim(), password, out SysUser user, out msg))
            {
                Log4NetHelper.WriteInfo(typeof(AccountController), $"Failed to log in {userName}");
                return base.FailedMsg(msg);
            }

            // 身份认证
            LoginInfoSession UserSession = new LoginInfoSession
            {
                UserId = user.UserId,
                RealName = user.RealName,
                DepartmentId = user.DepartmentId.ToString(),
                RoleId = user.RoleId.ToString(),
            };
            UserInfoSession = UserSession;
            Log4NetHelper.WriteInfo(typeof(AccountController), $"Logged in {userName}");
            return SuccessMsg(msg);
        }

        public IActionResult Logout()
        {
            //get方法会注销账号登陆
            UserInfoSession = null;
            return RedirectToAction("Login");
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}