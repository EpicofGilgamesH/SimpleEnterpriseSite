using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class RoleClaim : IEntityBase<Guid>
    {
        public Guid Id { get; set; }
        public Guid ClaimId { get; set; }
        public Guid RoleId { get; set; }
        public string BtnPermission { get; set; }
    }
}
