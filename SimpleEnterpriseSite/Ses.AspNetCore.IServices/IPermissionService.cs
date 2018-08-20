using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework;
using Ses.AspNetCore.Framework.Model.Permission;
using Ses.AspNetCore.Framework.Permission;
using Ses.AspNetCore.ViewModels.Permission;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.IServices
{
    public interface IPermissionService
    {
        /// <summary>
        /// 获取人员权限并集
        /// </summary>
        /// <param name="loginInfo">用户登陆信息</param>
        /// <returns></returns>
        List<SysClaim> GetUnionPermission(LoginInfoSession loginInfo);

        /// <summary>
        ///  获取人员权限并集
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="deptId">部门Id</param>
        /// <returns></returns>
        List<CompletePemission> GetUnionPermission(string userId);

        /// <summary>
        /// 获取人员按钮权限并集
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetUnionBtnPermission(LoginInfoSession loginInfo, string url);

        List<SysClaim> GetAllPermission();

        List<CompletePemission> GetUserPermission(string userid);

        List<CompletePemission> GetUserRolePermission(string roleId);

        List<CompletePemission> GetDeptPermission(string deptId);

        List<CompletePemission> GetRolePermission(string roleId);
        bool OperateUserPermission(string userId, List<PermissionData> permissionData);
        bool OperateRolePermission(string roleId, List<PermissionData> permissionData);
        bool OperateDeptPermission(string deptId, List<PermissionData> permissionData);

        /// <summary>
        /// 获取相应模块对应权限的人员列表
        /// </summary>
        /// <param name="claimId">页面（模块Id）</param>
        /// <param name="btnPermission">按钮权限</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageindex">页序号</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        List<ClaimUserModel> GetRolePermissionUsers(string claimId, string btnPermission, string userName,
            out int totalCount, int pageindex = 0, int pageSize = 10);
    }
}
