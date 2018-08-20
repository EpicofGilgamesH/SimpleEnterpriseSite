using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Helper.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.IServices
{
    public interface IModuleService
    {
        /// <summary>
        /// 获取菜单树形集合
        /// </summary>
        /// <param name="isSimple">是否为简单形势</param>
        /// <returns></returns>
        List<SesTreeModel> GetModuleList(string keyword = null);

        bool AddModule(SysClaim sysClaim);

        bool DeleteModule(string[] ids);

        bool UpdateModule(SysClaim sysClaim);

        bool IsHaveChlid(string[] ids);

        SysClaim GetClaimById(string id);
    }
}
