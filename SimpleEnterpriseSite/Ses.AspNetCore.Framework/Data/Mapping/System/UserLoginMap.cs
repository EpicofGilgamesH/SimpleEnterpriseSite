using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class UserLoginMap : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("UserLogin");
            builder.HasKey("Id");
            builder.Property(x => x.UserPassword).IsRequired();
            builder.Property(x => x.UserSecretkey).IsRequired();
            builder.Property(x => x.SysUserId).IsRequired();
        }
    }
}
