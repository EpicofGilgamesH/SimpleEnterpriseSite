using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ses.AspNetCore.IServices
{
    public interface IRoleService
    {
        List<RoleViewModel> GetRolePageList(int pagesize, int pageindex
            , out int totalcount, string role, string user);

        bool AddRole(SysRole sysRole, List<UserRole> list);

        bool UpdateRole(SysRole entity, List<Guid> userList);

        bool DeleteRoles(string[] ids);

        List<SysUser> GetRoleUserIds(string roleId);
    }
}
