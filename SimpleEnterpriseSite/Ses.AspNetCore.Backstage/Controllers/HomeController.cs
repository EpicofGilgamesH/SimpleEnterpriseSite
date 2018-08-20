using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.Backstage.Models;
using Ses.AspNetCore.Framework.IRepository;
using Ses.AspNetCore.Entities;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Web_Controller;
using Ses.AspNetCore.IServices;
using System.Text;
using Newtonsoft.Json;

namespace Ses.AspNetCore.Backstage.Controllers
{
    public class HomeController : WebController
    {
        private IPermissionService _perissionService;
        public HomeController(IPermissionService permissionService)
        {
            _perissionService = permissionService;
        }
        public IActionResult Index()
        {           
            ViewBag.CurrentSession = UserInfoSession;
            return View();
        }

        [HttpGet]
        public IActionResult GetMenusJsonData()
       {
            var rootDetpId = Guid.Empty;
            var menuList = _perissionService.GetUnionPermission(UserInfoSession);
            menuList.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.Url))
                    x.Url = Url.Content(x.Url);
            });
            var data = new
            {
                authorizeMenu = GetMenuJson(menuList, rootDetpId)
            };
            return SuccessData(data);
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private string GetMenuJson(List<SysClaim> list, Guid parentId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            List<SysClaim> entities = list.FindAll(t => t.ParentId == parentId);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    string jsonStr = JsonConvert.SerializeObject(item);
                    jsonStr = jsonStr.Insert(jsonStr.Length - 1, ",\"ChildNodes\":" + GetMenuJson(list, item.Id) + "");
                    stringBuilder.Append(jsonStr + ",");
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }
}
