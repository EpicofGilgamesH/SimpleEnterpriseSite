using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.Framework.IService;
using Ses.AspNetCore.Entities.System;
using AutoMapper;
using Ses.AspNetCore.Framework.Web_Controller;
using Ses.AspNetCore.IServices;
using System.Linq.Expressions;
using Ses.AspNetCore.Backstage.Filter;
using Ses.AspNetCore.ViewModels.RoleManage;

namespace Ses.AspNetCore.Backstage.Controllers
{
    /// <summary>
    /// 角色管理控制器 
    /// </summary>
    public class RoleController : WebController
    {
        private IRoleService _roleService;
        private IUserManageService _userService;
        private IMapper _mapper;

        public RoleController(IRoleService roleService, IUserManageService userService
            , IMapper mapper)
        {
            _roleService = roleService;
            _userService = userService;
            _mapper = mapper;
        }

        [ClaimPermission]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PageIndex(string roleName, string userId, int pagesize = 10, int pageindex = 0)
        {
            var pageList = _roleService.GetRolePageList(pagesize, pageindex, out int totalcount, roleName, userId);
            return JsonContent(new { rows = pageList, total = totalcount });
        }

        public IActionResult GetRoleItems()
        {
            IBaseService<SysRole, Guid> baseService = _roleService as IBaseService<SysRole, Guid>;
            var roles = baseService.Get(x => true);
            return JsonContent(roles.Select(x => new { text = x.Name, value = x.Id }));
        }

        /// <summary>
        /// 添加角色+角色成员
        /// </summary>
        /// <returns></returns>
        public IActionResult AddRole(AddRoleViewModel addRole)
        {
            IBaseService<SysRole, Guid> baseService = _roleService as IBaseService<SysRole, Guid>;
            var isExists = baseService.Exists(x => x.Name == addRole.RoleId.Trim());
            if (isExists)
                return FailedMsg("该角色名已存在");
            SysRole sysRole = new SysRole
            {
                Id = Guid.NewGuid(),
                Name = addRole.RoleId,
                NormalizedName = addRole.RoleInfo,
                State = addRole.RoleState,
                CreateUserId = UserInfoSession.UserId,
                CreationTime = DateTime.Now
            };
            List<UserRole> userRoles = new List<UserRole>();
            var userStr = addRole.RoleUsers;
            var userArray = userStr.Split(',');
            var ids = userArray.Select(x => x.Substring(0, x.IndexOf('(')));
            var userList = (_userService.GetUIdList(ids.ToList()));

            foreach (var item in userList)
            {
                userRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),
                    RoleId = sysRole.Id,
                    UserId = item
                });
            }

            var flag = _roleService.AddRole(sysRole, userRoles);
            if (flag)
                return AddSuccessMsg();
            return FailedMsg("添加角色失败");
        }

        public IActionResult UpdateRole(AddRoleViewModel updateRole)
        {
            IBaseService<SysRole, Guid> baseService = _roleService as IBaseService<SysRole, Guid>;
            var entity = baseService.Get(x => x.Name == updateRole.RoleId).FirstOrDefault();
            if (entity == null)
                return FailedMsg("更新出错,该角色不存在");
            entity.NormalizedName = updateRole.RoleInfo;
            entity.State = updateRole.RoleState;
            entity.LastModifyUserId = UserInfoSession.UserId;
            entity.LastModifyTime = DateTime.Now;

            var userStr = updateRole.RoleUsers;
            var userArray = userStr.Split(',');
            var ids = userArray.Select(x => x.Substring(0, x.IndexOf('(')));
            var userList = (_userService.GetUIdList(ids.ToList()));

            var flag = _roleService.UpdateRole(entity, userList);
            if (flag)
                return UpdateSuccessMsg();
            return FailedMsg("添加角色失败");
        }


        public IActionResult DeleteByIds(string[] ids)
        {
            try
            {
                _roleService.DeleteRoles(ids);
                return DeleteSuccessMsg();
            }
            catch (Exception ex)
            {
                return FailedMsg("删除用户失败" + ex.Message);
            }

        }

        public IActionResult GetRoleById(string roleId)
        {
            IBaseService<SysRole, Guid> baseService = _roleService as IBaseService<SysRole, Guid>;
            var entity = baseService.Get(x => x.Id.ToString() == roleId.Trim()).FirstOrDefault();
            return JsonContent(entity);
        }

        public IActionResult GetRoleUserIds(string roleId)
        {
            //IBaseService<SysUser, Guid> baseService = _userService as IBaseService<SysUser, Guid>;
            //var list = baseService.Get(x => x.RoleId.ToString() == roleId.Trim());
            var ids = _roleService.GetRoleUserIds(roleId.Trim());
            return JsonContent(ids);
        }
    }
}