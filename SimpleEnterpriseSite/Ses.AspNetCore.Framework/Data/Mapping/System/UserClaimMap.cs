using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class UserClaimMap : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable("UserClaim");
            builder.HasKey(x => x.Id);
            //外键  1->1
            builder.Property(x => x.ClaimId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
