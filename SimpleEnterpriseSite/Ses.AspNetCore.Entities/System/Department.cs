using Ses.AspNetCore.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class Department : IEntityBase<Guid>
    {
        public Department() => State = StateEnum.Normal;
        public Guid Id { get; set; }
        public Guid Pid { get; set; }
        public bool IsParent { get; set; }
        public string EnCode { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public int? SortCode { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreateUserId { get; set; }
        public StateEnum State { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleteUserId { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUserId { get; set; }

    }
}
