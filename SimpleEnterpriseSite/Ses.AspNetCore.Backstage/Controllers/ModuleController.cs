using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.IServices;
using Ses.AspNetCore.Framework.Web_Controller;
using Ses.AspNetCore.Backstage.Filter;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.ViewModels.Module;
using AutoMapper;

namespace Ses.AspNetCore.Backstage.Controllers
{
    public class ModuleController : WebController
    {
        private IModuleService _moduleService;
        private IMapper _mapper;

        public ModuleController(IModuleService moduleService, IMapper mapper)
        {
            _moduleService = moduleService;
            _mapper = mapper;
        }

        [ClaimPermission]
        public IActionResult Index(string keyword)
        {
            var data = _moduleService.GetModuleList(keyword);
            ViewBag.SearchKey = keyword;
            return View(data);
        }


        public IActionResult GetClaimOptions()
        {
            var data = _moduleService.GetModuleList();
            var result = data.Select(x => new { text = GetPrevSpan(x.Level) + ((SysClaim)x.Data).Name, value = x.Id });
            return JsonContent(result);
        }


        public IActionResult AddClaim(ModuleViewModel addEntity)
        {
            var entity = _mapper.Map<SysClaim>(addEntity);
            entity.CreateTime = DateTime.Now;
            entity.CreateUserId = UserInfoSession.UserId;
            if (_moduleService.AddModule(entity))
                return AddSuccessMsg();
            return FailedMsg("添加菜单失败");
        }

        public IActionResult EditClaim(ModuleViewModel editEntity)
        {
            var entity = _mapper.Map<SysClaim>(editEntity);
            entity.LastModifyTime = DateTime.Now;
            entity.LastModifyUserId = UserInfoSession.UserId;
            if (_moduleService.UpdateModule(entity))
                return UpdateSuccessMsg();
            return FailedMsg("更新菜单失败");
        }

        public IActionResult DeleteClaims(string[] ids)
        {
            if (_moduleService.IsHaveChlid(ids))
                return FailedMsg("勾选项中包含有子菜单的项");
            if (_moduleService.DeleteModule(ids))
                return DeleteSuccessMsg();
            return FailedMsg("删除菜单失败");
        }

        public IActionResult GetClaimById(string id)
        {
            var data = _moduleService.GetClaimById(id);
            return JsonContent(data);
        }

        private string GetPrevSpan(int level)
        {
            string prevSpan = "　";
            for (int i = 0; i < level; i++)
            {
                prevSpan += prevSpan;
            }
            return prevSpan;
        }

    }
}