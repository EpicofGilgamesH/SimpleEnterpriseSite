using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ses.AspNetCore.Backstage.Components
{
    public class PermissionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string target)
        {
            return View(target);
        }
    }
}
