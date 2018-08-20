using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    /// <summary>
    /// 用户-权限关系表
    /// </summary>
    public class UserClaim : IEntityBase<Guid>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ClaimId { get; set; }
        public string BtnPermission { get; set; }
    }
}
