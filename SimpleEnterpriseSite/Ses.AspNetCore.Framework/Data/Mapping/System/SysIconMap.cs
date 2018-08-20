using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public partial class SysIconMap : IEntityTypeConfiguration<SysIcon>
    {
        public void Configure(EntityTypeBuilder<SysIcon> builder)
        {
            builder.ToTable("SysIcon");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Icon).IsRequired();
        }
    }
}
