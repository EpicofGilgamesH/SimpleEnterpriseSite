using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using Ses.AspNetCore.Framework.Helper.Tree;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.IRepository;
using System.Linq;
using Ses.AspNetCore.Entities.Enum;

namespace Ses.AspNetCore.Services
{
    public class ModuleService : IModuleService
    {
        private static string _defaultId = Guid.Empty.ToString();
        private IRepository<SysClaim, Guid> _repository;
        public ModuleService(IRepository<SysClaim, Guid> repository)
        {
            _repository = repository;
        }

        public List<SesTreeModel> GetModuleList(string keyword = null)
        {

            List<SesTreeModel> list = new List<SesTreeModel>();
            var data = _repository.Entities.ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = TreeHelp.TreeWhere(data, x => x.Name.Contains(keyword.Trim()), x => x.Id.ToString(), x => x.ParentId.ToString());
            }
            TreeHelp.CreateTreeModel(data, _defaultId, 0, x => x.Id.ToString(), x => x.ParentId.ToString(), ref list, x => x.Sort);
            return list;
        }

        public bool AddModule(SysClaim sysClaim)
        {
            var a = _repository.Add(sysClaim);
            return a == -1 ? false : true;
        }

        public bool DeleteModule(string[] ids)
        {
            var deletes = _repository.Get(x => ids.Contains(x.Id.ToString())).ToList();
            return _repository.Transcation(context =>
             {
                 context.RemoveRange(deletes);
                 context.SaveChanges();
             });
        }

        public bool UpdateModule(SysClaim sysClaim)
        {
            var a = _repository.Edit(sysClaim);
            return a == -1 ? false : true;
        }


        public bool IsHaveChlid(string[] ids)
        {
            var entities = _repository.Get(x => ids.Contains(x.Id.ToString())).ToList();
            int count = _repository.Entities.Where(x => ids.Contains(x.ParentId.ToString())).Count();
            return count > 0;
        }

        public SysClaim GetClaimById(string id)
        {
            var entity = _repository.Get(x => x.Id.ToString() == id).FirstOrDefault();
            return entity;
        }
    }
}
