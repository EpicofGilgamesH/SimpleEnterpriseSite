using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class SysIcon : IEntityBase<Guid>
    {
        public Guid Id { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public int Order { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
