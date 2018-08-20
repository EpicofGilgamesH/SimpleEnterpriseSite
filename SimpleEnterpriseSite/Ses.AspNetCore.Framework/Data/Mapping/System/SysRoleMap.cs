using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class SysRoleMap : IEntityTypeConfiguration<SysRole>
    {
        public void Configure(EntityTypeBuilder<SysRole> builder)
        {
            builder.ToTable("SysRole");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(256);
            builder.Property(x => x.NormalizedName).HasMaxLength(256);
            builder.Property(x => x.ConcurrencyStamp).HasMaxLength(256);
        }
    }
}
