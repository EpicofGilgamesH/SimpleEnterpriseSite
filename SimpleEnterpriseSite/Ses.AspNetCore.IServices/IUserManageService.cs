using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.ViewModels.UserManage;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ses.AspNetCore.IServices
{
    public interface IUserManageService
    {
        /// <summary>
        /// 用户分页数据
        /// </summary>
        /// <param name="pagesize">页面数量</param>
        /// <param name="pageindex">页码</param>
        /// <param name="totalcount">总页数</param>
        /// <param name="searchExpression">查询条件Func</param>
        /// <param name="orderExpression"></param>
        /// <returns></returns>
        List<UserPageViewModel> GetRolePageList(int pagesize,
            int pageindex, out int totalcount,
            List<Expression<Func<SysUser, bool>>> searchExpression,
            Dictionary<string, bool> orderDic);

        bool AddUserAndLoinInfo(SysUser model, string UserId);

        bool DeleteUser(string[] ids);

        List<Guid> GetUIdList(List<string> guids);


        string GetDeptEncode(string guid);
    }
}
