using Ses.AspNetCore.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ses.AspNetCore.ViewModes
{
    public class UserInputBase
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string DepartmentId { get; set; }
        public string DutyId { get; set; }
        public string MobilePhone { get; set; }
        public DateTime? Birthday { get; set; }
        public string WeChat { get; set; }
        public string Email { get; set; }
        public StateEnum State { get; set; }
        public string Description { get; set; }
    }
}
