using Ses.AspNetCore.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Entities.System
{
    /// <summary>
    /// 权限模块
    /// </summary>
    public class SysClaim : IEntityBase<Guid>
    {
        public SysClaim() => State = StateEnum.Normal;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Guid ParentId { get; set; }
        public string Url { get; set; }
        public int Sort { get; set; }
        public bool IsMenu { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUserId { get; set; }
        public DateTime? LastModifyTime { get; set; }
        public string LastModifyUserId { get; set; }
        public StateEnum State { get; set; }
        public DateTime? DeleteTime { get; set; }
        public string DeleteUserId { get; set; }
    }
}
