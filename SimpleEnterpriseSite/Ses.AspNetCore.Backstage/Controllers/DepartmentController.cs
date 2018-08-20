using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.Framework.Web_Controller;
using Ses.AspNetCore.IServices;
using System.Linq.Expressions;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Entities.Enum;
using Ses.AspNetCore.Framework.IService;
using Ses.AspNetCore.Backstage.Filter;

namespace Ses.AspNetCore.Backstage.Controllers
{

    public class DepartmentController : WebController
    {
        private IDeptManageService _deptManageService;
        private IBaseService<SysUser, Guid> _userService;
        private readonly Guid defaulGuid = new Guid("00000000-0000-0000-0000-000000000000");
        public DepartmentController(IDeptManageService deptManageService,
            IBaseService<SysUser, Guid> userService)
        {
            _deptManageService = deptManageService;
            _userService = userService;
        }

        [ClaimPermission]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PageIndex(string deptName, string detpSearchId,
            int? state, int pageindex = 0, int pagesize = 10)
        {
            List<Expression<Func<Department, bool>>> searchs =
                new List<Expression<Func<Department, bool>>>();
            if (!string.IsNullOrEmpty(deptName))
                searchs.Add(x => x.Name.Contains(deptName));
            if (state != null && state != -1)
            {
                StateEnum stateEnum = (StateEnum)state;
                searchs.Add(x => x.State == stateEnum);
            }
            if (!string.IsNullOrEmpty(detpSearchId))
            {
                IBaseService<Department, Guid> deptBaseService = _deptManageService as IBaseService<Department, Guid>;
                var entity = deptBaseService.Get(x => x.Id.ToString() == detpSearchId).FirstOrDefault();
                if (entity != null)
                {
                    searchs.Add(x => x.EnCode.StartsWith(entity.EnCode));
                }
            }
            Dictionary<string, bool> orderDic = new Dictionary<string, bool>
           {
                {"SortCode",true }
            };
            var pageList = _deptManageService.GetDeptPageList(pagesize, pageindex,
                out int totalcount, searchs, orderDic);
            return JsonContent(new { rows = pageList, total = totalcount });
        }


        public IActionResult AddDept(string pid, string name, bool isParent)
        {
            IBaseService<Department, Guid> deptBaseService = _deptManageService as IBaseService<Department, Guid>;
            Guid guidPid;
            var enCode = string.Empty;
            if (pid == null) //添加的是根节点
            {
                guidPid = defaulGuid;
                enCode = "0.";
            }
            else
            {
                guidPid = new Guid(pid);
                var pEnCode = deptBaseService.Get(x => x.Id == guidPid).Select(x => x.EnCode).FirstOrDefault();
                enCode = pEnCode + (deptBaseService.Get(x => x.Pid == guidPid).Count() + 1).ToString() + ".";
            }

            Department department = new Department
            {
                Name = name,
                Pid = guidPid,
                EnCode = enCode,
                CreateUserId = UserInfoSession.UserId,
                CreationTime = DateTime.Now,
                IsParent = isParent,
            };
            var code = deptBaseService.Add(department);
            if (code == 1)
                return AddSuccessData(department.Id);
            return FailedMsg("添加部门失败");
        }

        public IActionResult EditDept(Guid? id, string name)
        {
            IBaseService<Department, Guid> deptBaseService = _deptManageService as IBaseService<Department, Guid>;
            var entity = deptBaseService.Get(x => x.Id == id).FirstOrDefault();
            if (entity != null)
            {
                entity.Name = name;
                entity.LastModifyUserId = UserInfoSession.UserId;
                entity.LastModifyTime = DateTime.Now;
                var code = deptBaseService.Edit(entity);
                if (code == 1)
                    return UpdateSuccessMsg();
                return FailedMsg("更新部门名称失败");
            }
            return FailedMsg("该部门不存在");
        }

        public IActionResult DeleteDept(Guid? id)
        {
            IBaseService<Department, Guid> deptBaseService = _deptManageService as IBaseService<Department, Guid>;
            if (deptBaseService.Get(x => x.Pid == id && x.State == StateEnum.Normal).Count() > 0)
                return FailedMsg("该部门下还有子部门");
            if (_userService.Get(x => x.DepartmentId == id).Count() > 0)
                return FailedMsg("请先处理该部门的成员，再进行删除");
            var entity = deptBaseService.Get(x => x.Id == id).FirstOrDefault();
            if (entity != null)
            {
                //if (entity.Pid == defaulGuid)
                //    return FailedMsg("根节点不能删除");
                entity.State = StateEnum.Delete;
                var code = deptBaseService.Edit(entity);
                if (code == 1)
                    return DeleteSuccessMsg();
                return FailedMsg("删除部门名称失败");
            }
            return FailedMsg("该部门不存在");
        }

        public IActionResult OrgnizitonItem()
        {
            IBaseService<Department, Guid> deptBaseService = _deptManageService as IBaseService<Department, Guid>;
            var test = deptBaseService.Get(x => x.State == StateEnum.Normal).OrderBy(x => x.SortCode).ToList();
            var data = deptBaseService.Get(x => x.State == StateEnum.Normal).OrderBy(x => x.SortCode).
                Select(x => new
                {
                    id = x.Id,
                    pId = x.Pid,
                    name = x.Name,
                    open = x.Pid == defaulGuid ? "true" : "",
                    isParent = (x.Pid == defaulGuid) || x.IsParent,
                }).ToList();

            return JsonContent(data);
        }

        public IActionResult EditParent(Guid? id)
        {
            IBaseService<Department, Guid> deptBaseService = _deptManageService as IBaseService<Department, Guid>;
            var entity = deptBaseService.Get(x => x.Id == id).FirstOrDefault();
            if (entity != null)
            {
                var code = deptBaseService.Edit(entity);
                if (code == 1)
                    return UpdateSuccessMsg();
                else
                    return FailedMsg("更新失败");
            }
            return FailedMsg("该部门不存在");
        }

        public IActionResult EditDeptForIsParent(Guid? id)
        {
            IBaseService<Department, Guid> deptBaseService = _deptManageService as IBaseService<Department, Guid>;
            var entity = deptBaseService.Get(x => x.Id == id).FirstOrDefault();
            if (entity != null)
            {
                entity.IsParent = !entity.IsParent;
                entity.LastModifyUserId = UserInfoSession.UserId;
                entity.LastModifyTime = DateTime.Now;
                var code = deptBaseService.Edit(entity);
                if (code == 1)
                    return JsonContent(new { status = true, flag = entity.IsParent, msg = "更新成功" });
                return JsonContent(new { status = false, msg = "更新部门名称失败" });
            }
            return JsonContent(new { status = false, msg = "该部门不存在" });
        }

    }
}