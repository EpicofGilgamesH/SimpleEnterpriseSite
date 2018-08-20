using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class UserRoleMap : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRole");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RoleId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
