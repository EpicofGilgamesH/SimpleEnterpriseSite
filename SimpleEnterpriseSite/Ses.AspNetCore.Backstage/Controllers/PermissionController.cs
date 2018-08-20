using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.Framework.Web_Controller;
using Ses.AspNetCore.IServices;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Model.Permission;
using Ses.AspNetCore.Framework.Permission;
using Ses.AspNetCore.Backstage.Filter;
using Ses.AspNetCore.Framework;

namespace Ses.AspNetCore.Backstage.Controllers
{
    public class PermissionController : WebController
    {
        private IPermissionService _permissionService;
        private readonly Guid defaulGuid = new Guid("00000000-0000-0000-0000-000000000000");
        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取当前用户权限树
        /// </summary>
        /// <param name="dynamic"></param>
        /// <param name="type"></param>
        /// <param name="isUserAllPermission"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PermissionTreeJson(string dynamic, string type, bool isUserAllPermission = false)
        {
            // 所有菜单列表
            var permission = _permissionService.GetAllPermission();
            List<CompletePemission> currentClaims = null;
            // 对于权限菜单列表
            switch (type)
            {
                case "dept":
                    currentClaims = _permissionService.GetDeptPermission(dynamic);
                    break;
                case "role":
                    currentClaims = _permissionService.GetRolePermission(dynamic);
                    break;
                case "user":
                    if (isUserAllPermission)
                        currentClaims = _permissionService.GetUnionPermission(dynamic);
                    else
                        currentClaims = _permissionService.GetUserPermission(dynamic);
                    break;
            }

            if (isUserAllPermission)
                return JsonContent(currentClaims.Select(x => new
                {
                    id = x.SysClaim.Id,
                    name = x.SysClaim.Name,
                    pId = x.SysClaim.ParentId,
                    open = x.SysClaim.ParentId == defaulGuid ? true : false,
                    myAttr = x.BtnPermission
                }));
            else
                return JsonContent(permission.Select(x => new
                {
                    id = x.Id,
                    name = x.Name,
                    pId = x.ParentId,
                    open = x.ParentId == defaulGuid ? true : false,
                    @checked = currentClaims == null ? false
                        : currentClaims.Any(y => y.SysClaim.Id == x.Id),
                    myAttr = currentClaims == null ? string.Empty
                       : currentClaims.Where(y => y.SysClaim.Id == x.Id)
                       .Select(z => z.BtnPermission).FirstOrDefault()
                }));
        }

        /// <summary>
        /// 保存权限设置操作
        /// </summary>
        /// <param name="dynamic"></param>
        /// <param name="type"></param>
        /// <param name="pemission"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult OperateSubmit(string dynamic, string type, List<PermissionData> pemission)
        {
            bool flag = false;
            switch (type)
            {
                case "dept":
                    flag = _permissionService.OperateDeptPermission(dynamic, pemission);
                    break;
                case "role":
                    flag = _permissionService.OperateRolePermission(dynamic, pemission);
                    break;
                case "user":
                    flag = _permissionService.OperateUserPermission(dynamic, pemission);
                    break;
            }
            if (flag)
                return SuccessMsg("设置成功");
            else
                return FailedMsg("设置失败");
        }

        /// <summary>
        /// 获取页面权限的所有用户
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PermissionUsers(string claimId, string btn, string username, int pageindex = 0, int pagesize = 10)
        {
            var pageList = _permissionService.GetRolePermissionUsers(claimId, btn, username, out int totalcount, pageindex, pagesize);
            return JsonContent(new { rows = pageList, total = totalcount });

        }

    }



}