using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.Framework.Web_Controller;
using System.Linq.Expressions;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.IServices;
using Ses.AspNetCore.ViewModes;
using AutoMapper;
using Ses.AspNetCore.Framework.IService;
using Ses.AspNetCore.Backstage.Filter;

namespace Ses.AspNetCore.Backstage.Controllers
{
    public class UserManageController : WebController
    {
        private IUserManageService _userManageService;
        private IBaseService<Department, Guid> _baseDeptService;
        private IBaseService<UserRole, Guid> _baseURService;
        private IMapper _mapper;
        public UserManageController(IUserManageService userManageService,
            IBaseService<Department, Guid> baseDeptService,
             IBaseService<UserRole, Guid> baseURService,
            [FromServices] IMapper mapper)
        {
            _userManageService = userManageService;
            _baseDeptService = baseDeptService;
            _baseURService = baseURService;
            _mapper = mapper;
        }

        [ClaimPermission]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 用户分页action
        /// </summary>
        /// <returns></returns>
        public IActionResult PageIndex(string realName, string deptId,
            string userId, int pageindex = 0, int pagesize = 10)
        {
            string enConde = string.Empty;
            if (!string.IsNullOrEmpty(deptId))
            {
                enConde = _userManageService.GetDeptEncode(deptId.Trim());
            }
            List<Expression<Func<SysUser, bool>>> searchs = new List<Expression<Func<SysUser, bool>>>();
            if (!string.IsNullOrEmpty(realName))
                searchs.Add(x => x.RealName.Contains(realName.Trim()));
            if (!string.IsNullOrEmpty(userId))
                searchs.Add(x => x.UserId.Contains(userId.Trim()));
            if (!string.IsNullOrEmpty(enConde))
                searchs.Add(x => x.Encode.StartsWith(enConde));
            Dictionary<string, bool> order = new Dictionary<string, bool>
            {
                { "CreationTime", true }
            };
            var pageList = _userManageService.GetRolePageList(pagesize, pageindex, out int totalcount, searchs, order);
            return JsonContent(new { rows = pageList, total = totalcount });
        }

        public IActionResult AddUser(AddUserViewModel addUser)
        {
            SysUser sysUser = _mapper.Map<AddUserViewModel, SysUser>(addUser);
            IBaseService<SysUser, Guid> baseService = _userManageService as IBaseService<SysUser, Guid>;
            if (baseService.Get(x => x.UserId.ToUpper() == addUser.UserId.ToUpper()).Count() > 0)
                return FailedMsg("该UserId已存在");
            string encode = string.Empty;
            if (sysUser.DepartmentId != null)
            {
                encode = _baseDeptService.Get(x => x.Id == sysUser.DepartmentId).Select(x => x.EnCode).FirstOrDefault();
            }
            sysUser.Encode = encode;
            var flag = _userManageService.AddUserAndLoinInfo(sysUser, UserInfoSession.UserId);
            if (flag)
                return AddSuccessMsg();
            else
                return FailedMsg("添加失败");
        }

        public IActionResult UpdateUser(AddUserViewModel updateUser)
        {
            SysUser sysUser = _mapper.Map<AddUserViewModel, SysUser>(updateUser);
            IBaseService<SysUser, Guid> baseService = _userManageService as IBaseService<SysUser, Guid>;
            var userEntity = baseService.Get(x => x.Id == new Guid(updateUser.Id)).FirstOrDefault();
            if (userEntity.DepartmentId != sysUser.DepartmentId)
            {
                string encode = string.Empty;
                if (sysUser.DepartmentId != null)
                {
                    encode = _baseDeptService.Get(x => x.Id == sysUser.DepartmentId).Select(x => x.EnCode).FirstOrDefault();
                }
                sysUser.Encode = encode;
            }
            sysUser.CreateUserId = userEntity.CreateUserId;
            sysUser.CreationTime = userEntity.CreationTime;
            sysUser.LastModifyUserId = UserInfoSession.UserId;
            sysUser.LastModifyTime = DateTime.Now;
            var flag = baseService.Edit(sysUser);
            if (flag == 1)
                return UpdateSuccessMsg();
            else
                return FailedMsg("更新失败");
        }


        public IActionResult GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return JsonContent(null);
            Guid id = new Guid(userId);
            IBaseService<SysUser, Guid> baseService = _userManageService as IBaseService<SysUser, Guid>;
            var userEntity = baseService.Get(x => x.Id == id).FirstOrDefault();
            string departmentName = string.Empty;
            if (userEntity.DepartmentId != null)
            {
                var dept = _baseDeptService.Get(x => x.Id == userEntity.DepartmentId).FirstOrDefault();
                if (dept != null)
                    departmentName = dept.Name;
            }
            return JsonContent(new { user = userEntity, departmentName = departmentName });
        }

        public IActionResult GetUserByNameOrId(string query)
        {
            if (string.IsNullOrEmpty(query))
                return JsonContent(null);
            IBaseService<SysUser, Guid> baseService = _userManageService as IBaseService<SysUser, Guid>;
            var user = baseService.Get(x => x.UserId.Contains(query.Trim()) || x.RealName.Contains(query.Trim())).ToList();
            if (user != null)
                return JsonContent(new { data = user });
            return JsonContent(null);
        }

        /// <summary>
        /// 用户删除(用户-->角色用户,要实现级联删除)
        /// 需要同时删除角色中包含该人员的 对应数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IActionResult DeleteByIds(string[] ids)
        {
            var flag = false;
            try
            {
                flag = _userManageService.DeleteUser(ids);
            }
            catch (Exception ex)
            {
                return FailedMsg("删除用户失败" + ex.Message);
            }
            if (flag)
                return SuccessMsg("删除用户成功");
            return FailedMsg("删除用户失败");
        }
    }
}