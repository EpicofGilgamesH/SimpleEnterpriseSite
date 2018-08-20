using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ses.AspNetCore.IServices
{
    public interface IIconService
    {
        IList<SysIcon> PageList(int pagesize,
            int pageindex, out int totalcount,
            List<Expression<Func<SysIcon, bool>>> searchExpression,
            Dictionary<string, bool> orderDic, bool isExport = false);

        bool AddIcon(SysIcon entity);

        bool UpdateIcon(SysIcon entity);

        bool DeleteIcon(Guid[] key);

        bool IsExsitIcon(string iconNo, Guid iconId);

        SysIcon GetById(Guid id);
    }
}
