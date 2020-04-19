using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Config
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("t_role");
            builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
            builder.Property(r => r.IsDelete).IsRequired();

        }
    }
}
