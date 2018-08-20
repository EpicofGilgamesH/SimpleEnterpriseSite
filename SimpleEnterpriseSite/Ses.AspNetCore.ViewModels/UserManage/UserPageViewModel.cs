using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.ViewModels.UserManage
{
    public class UserPageViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RealName { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string WeChat { get; set; }
        public string State { get; set; }
        public string CreationTime { get; set; }
        public string Description { get; set; }
        public string DepartmentName { get; set; }
    }
}
