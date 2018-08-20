using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ses.AspNetCore.Entities.System;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    /// <summary>
    /// Fluent Api 规定实体间的映射关系
    /// </summary>
    public partial class SysUserMap : IEntityTypeConfiguration<SysUser>
    {
        public void Configure(EntityTypeBuilder<SysUser> builder)
        {
            builder.ToTable("SysUser");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(256);
            builder.Property(x => x.RealName).IsRequired().HasMaxLength(256);
            builder.Property(x => x.State).IsRequired();
        }
    }
}
