using Ses.AspNetCore.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.ViewModels.RoleManage
{
    public class AddRoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleInfo { get; set; }
        public StateEnum RoleState { get; set; }
        public string RoleUsers { get; set; }
    }
}
