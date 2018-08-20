using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Service;
using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Ses.AspNetCore.Framework.IRepository;
using Ses.AspNetCore.Framework.Extesions;
using System.Linq;
using Ses.AspNetCore.ViewModels.Department;

namespace Ses.AspNetCore.Services
{
    public class DeptManageService : BaseService<Department, Guid>, IDeptManageService
    {
        public DeptManageService(IRepository<Department, Guid> repository) : base(repository)
        {
        }

        public List<DepartmentPageListViewModel> GetDeptPageList(int pagesize,
            int pageindex, out int totalcount,
            List<Expression<Func<Department, bool>>> searchExpression,
            Dictionary<string, bool> orderDic)
        {
            IQueryable<Department> queryable = _repository.Entities;
            queryable = queryable.GetTableQueryable(searchExpression, orderDic, out totalcount);

            var result = from x in queryable
                         join y in _repository.Entities
                         on x.Pid equals y.Id into temp
                         from z in temp.DefaultIfEmpty()
                         select new DepartmentPageListViewModel
                         {
                             Id = x.Id.ToString(),
                             Name = x.Name,
                             Manager = x.Manager,
                             PName = z.Name,
                             IsParent = x.IsParent.ToString(),
                             State = x.State.ToString(),
                             CreationTime = x.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                             Description = x.Description
                         };
            return result.GetPageList(pagesize, pageindex).ToList();
        }
    }
}
