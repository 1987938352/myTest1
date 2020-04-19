using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Config
{
    public class RoleUserConfig : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("t_userrole");
            builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).IsRequired();
            builder.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId).IsRequired();
        }
    }
}
