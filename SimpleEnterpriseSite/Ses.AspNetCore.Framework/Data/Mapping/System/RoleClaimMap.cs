using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class RoleClaimMap : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("RoleClaim");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ClaimId).IsRequired();
            builder.Property(x => x.RoleId).IsRequired();
        }
    }
}
