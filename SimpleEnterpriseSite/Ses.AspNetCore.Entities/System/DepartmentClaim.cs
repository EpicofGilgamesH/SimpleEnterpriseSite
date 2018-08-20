using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class DepartmentClaim : IEntityBase<Guid>
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid ClaimId { get; set; }
        public string BtnPermission { get; set; }
    }
}
