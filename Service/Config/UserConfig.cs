using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Config
{
    class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("T_User");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(20);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(50);
            builder.Property(e => e.PassWord).IsRequired().HasMaxLength(50);
            builder.Property(e => e.IsDelete).IsRequired();
        }
    }
}
