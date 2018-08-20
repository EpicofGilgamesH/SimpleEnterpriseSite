using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ses.AspNetCore.IServices
{
    public interface IDeptManageService
    {
        List<DepartmentPageListViewModel> GetDeptPageList(int pagesize,
        int pageindex, out int totalcount,
        List<Expression<Func<Department, bool>>> searchExpression,
        Dictionary<string, bool> orderDic);
    }
}
