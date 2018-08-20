using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class SysClaimMap : IEntityTypeConfiguration<SysClaim>
    {
        public void Configure(EntityTypeBuilder<SysClaim> builder)
        {
            builder.ToTable("SysClaim");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.State).IsRequired();
        }
    }
}
