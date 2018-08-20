using Microsoft.EntityFrameworkCore;
using Ses.AspNetCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ses.AspNetCore.Framework.Data.Mapping.System
{
    public class DepartmentMap : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Department");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Pid).IsRequired();
        }
    }
}
