using Ses.AspNetCore.Entities.Enum;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class SysUser : IEntityBase<Guid>
    {
        public SysUser() => State = StateEnum.Normal;

        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string RealName { get; set; }
        public string HeadIcon { get; set; }
        public string Encode { get; set; }
        public GenderEnum? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string WeChat { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? RoleId { get; set; }
        public string DutyId { get; set; }
        public string Description { get; set; }
        public DateTime? CreationTime { get; set; }
        public string CreateUserId { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUserId { get; set; }
        public StateEnum State { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleteUserId { get; set; }

    }
}
