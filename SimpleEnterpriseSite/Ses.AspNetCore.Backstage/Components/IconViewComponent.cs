using Microsoft.AspNetCore.Mvc;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ses.AspNetCore.Backstage.Components
{
    public class IconViewComponent : ViewComponent
    {
        private IRepository<SysIcon, Guid> _repository;
        public IconViewComponent(IRepository<SysIcon, Guid> repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            var icons = _repository.Entities.OrderByDescending(x => x.UpdateTime).ThenBy(x => x.Order).ToList();
            return View(icons);
        }
    }
}
