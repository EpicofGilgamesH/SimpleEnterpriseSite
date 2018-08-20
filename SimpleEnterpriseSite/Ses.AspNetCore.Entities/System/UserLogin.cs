using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class UserLogin : IEntityBase<Guid>
    {
        public Guid Id { get; set; }
        public Guid SysUserId { get; set; }
        public string UserPassword { get; set; }
        public string UserSecretkey { get; set; }
        public DateTime? PreviousVisitTime { get; set; }
        public DateTime? LastVisitTime { get; set; }
        public int LogOnCount { get; set; }
    }
}
