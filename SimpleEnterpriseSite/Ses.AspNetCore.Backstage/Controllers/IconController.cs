using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Web_Controller;
using Ses.AspNetCore.ViewModels.Icon;
using AutoMapper;
using Ses.AspNetCore.Backstage.Filter;
using Ses.AspNetCore.IServices;
using Ses.AspNetCore.Framework.Helper.EPPlus.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Ses.AspNetCore.Framework.IService;

namespace Ses.AspNetCore.Backstage.Controllers
{
    /// <summary>
    /// 菜单管理控制器
    /// </summary>
    public class IconController : WebController
    {
        private IIconService _iconService;
        private IMapper _mapper;
        private IHostingEnvironment _hosting;

        private const string _exportName = "导出系统图标数据";

        private const string _importName = "导入系统图标数据";

        public IconController(IIconService iconService, IMapper mapper,
            IHostingEnvironment hosting)
        {
            _iconService = iconService;
            _mapper = mapper;
            _hosting = hosting;
        }

        /// <summary>
        /// 查询分页列表前，获取查询表达式和排序字典
        /// </summary>
        /// <param name="iconname"></param>
        /// <returns></returns>
        private Tuple<List<Expression<Func<SysIcon, bool>>>, Dictionary<string, bool>> GetPrecondition
            (string iconname)
        {
            var searchExp = new List<Expression<Func<SysIcon, bool>>>();
            if (!string.IsNullOrEmpty(iconname))
            {
                searchExp.Add(x => x.Icon.Contains(iconname.Trim()));
            }
            var orderFunc = new Dictionary<string, bool>
                  {
                    {"UpdateTime",true },
                    {"Icon",true },
                  };
            return Tuple.Create(searchExp, orderFunc);
        }


        [ClaimPermission]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取图标分页数据
        /// </summary>
        /// <param name="iconname"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PageIndex(string iconname,
            int pageindex = 0, int pagesize = 10)
        {
            var tuple = GetPrecondition(iconname);
            var pageList = _iconService.PageList(pagesize, pageindex, out int totalcount, tuple.Item1, tuple.Item2);
            return JsonContent(new { rows = pageList, total = totalcount });
        }

        [HttpPost]
        public IActionResult Export(string iconname)
        {
            var tuple = GetPrecondition(iconname);
            var exportList = _iconService.PageList(0, 0, out int totalcount, tuple.Item1, tuple.Item2, true);
            ExportExcelHelper<SysIcon> exprot = new ExportExcelHelper<SysIcon>(_hosting.WebRootPath);
            if (exprot.Export(_exportName, exportList, out string filepath, out string excelname))
                return File($"~\\{filepath}", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelname);
            else
                return FailedMsg("导出Excel失败 - " + filepath);

        }

        [HttpPost]
        public IActionResult Import(IFormFile formFile)
        {
            IBaseService<SysIcon, Guid> deptBaseService = _iconService as IBaseService<SysIcon, Guid>;
            ImportExcelHelper<SysIcon> import = new ImportExcelHelper<SysIcon>(_hosting.WebRootPath, deptBaseService);
            var filepath = import.Import(formFile, _importName, out string excelname);
            return File($"~\\{filepath}", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelname);
        }


        public IActionResult AddIcon(IconViewModel addModel)
        {
            var entity = _mapper.Map<SysIcon>(addModel);
            entity.UpdateTime = DateTime.Now;
            if (_iconService.AddIcon(entity))
                return AddSuccessMsg();
            return FailedMsg("添加失败");
        }

        public IActionResult UpdateIcon(IconViewModel addModel)
        {
            var entity = _mapper.Map<SysIcon>(addModel);
            if (_iconService.IsExsitIcon(entity.Icon, entity.Id))
                return FailedMsg("该编号已存在");
            entity.UpdateTime = DateTime.Now;
            if (_iconService.UpdateIcon(entity))
                return UpdateSuccessMsg();
            return FailedMsg("更新失败");
        }

        public IActionResult DeleteByIds(Guid[] ids)
        {
            if (_iconService.DeleteIcon(ids))
                return DeleteSuccessMsg();
            return FailedMsg("删除用户失败");
        }

        public IActionResult GetIconById(Guid id)
        {
            var entity = _iconService.GetById(id);
            if (entity != null)
                return JsonContent(entity);
            return FailedMsg("找不到该图标Id");
        }
    }
}