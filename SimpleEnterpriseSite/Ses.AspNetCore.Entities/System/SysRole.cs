using Ses.AspNetCore.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    public class SysRole : IEntityBase<Guid>
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 并发标识
        /// </summary>
        public string ConcurrencyStamp { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 标准化名称
        /// </summary>
        public string NormalizedName { get; set; }
        public DateTime? CreationTime { get; set; }
        public string CreateUserId { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUserId { get; set; }
        public StateEnum State { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleteUserId { get; set; }
    }
}
