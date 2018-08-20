using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class UserRole : IEntityBase<Guid>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
