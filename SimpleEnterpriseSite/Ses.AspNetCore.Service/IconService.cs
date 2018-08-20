using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.IRepository;
using System.Linq.Expressions;
using System.Linq;
using Ses.AspNetCore.Framework.Service;

namespace Ses.AspNetCore.Services
{
    public class IconService : BaseService<SysIcon, Guid>, IIconService
    {
        //private IRepository<SysIcon, Guid> _repository;
        public IconService(IRepository<SysIcon, Guid> repository):base(repository)
        {
            
        }
        public bool AddIcon(SysIcon entity)
        {
            return _repository.Add(entity) > 0;
        }

        public bool DeleteIcon(Guid[] keys)
        {
            var entities = _repository.Get(x => keys.Contains(x.Id)).ToList();
            return _repository.Transcation(context =>
             {
                 context.RemoveRange(entities);
                 context.SaveChanges();
             });
        }

        public IList<SysIcon> PageList(int pagesize, int pageindex,
            out int totalcount, List<Expression<Func<SysIcon, bool>>> searchExpression,
            Dictionary<string, bool> orderExpression, bool isExport = false)
        {
            IQueryable<SysIcon> queryable = _repository.Entities;
            try
            {
                queryable = queryable.GetTableQueryable(searchExpression, orderExpression, out totalcount);
                if (!isExport)
                    return queryable.GetPageList(pagesize, pageindex).ToList();
                else
                    return queryable.GetPageList(totalcount, 0).ToList();
            }
            catch
            {
                totalcount = 0;
                return null;
            }
        }

        public bool UpdateIcon(SysIcon entity)
        {
            return _repository.Edit(entity) > 0;
        }

        public bool IsExsitIcon(string iconNo, Guid iconId)
        {
            return _repository.Get(x => x.Icon == iconNo && x.Id != iconId).Count() > 0;
        }

        public SysIcon GetById(Guid id)
        {
            var entities = _repository.Get(x => x.Id == id);
            if (entities.Count() > 0)
                return entities.FirstOrDefault();
            return null;
        }
    }
}
