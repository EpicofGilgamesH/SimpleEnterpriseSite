using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class DepartmentClaimMap : IEntityTypeConfiguration<DepartmentClaim>
    {
        public void Configure(EntityTypeBuilder<DepartmentClaim> builder)
        {
            builder.ToTable("DepartmentClaim");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DepartmentId).IsRequired();
            builder.Property(x => x.ClaimId).IsRequired();
        }
    }
}
