using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.ViewModels.Permission
{
    public class ClaimUserModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string RealName { get; set; }
    }
}
