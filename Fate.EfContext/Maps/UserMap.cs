using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Users;

namespace Fate.EfContext.Maps
{
    public class UserMap : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.ToTable("UserInfo")
                .HasKey(c => c.ID);
            builder.Property(c => c.ID).IsRequired(true).HasColumnName(@"ID").HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(c => c.UserName).IsRequired(false).HasColumnName(@"UserName").HasMaxLength(20);
            builder.Property(c => c.UserPassword).IsRequired(false).HasColumnName(@"UserPassword").HasMaxLength(20);
            builder.Property(c => c.Phone).IsRequired(false).HasColumnName(@"Phone").HasMaxLength(20);
            builder.Property(c => c.Avatar).IsRequired(false).HasMaxLength(100);
            builder.Property(c => c.Name).IsRequired(false).HasMaxLength(100);
            builder.Property(c => c.Company).IsRequired(false).HasMaxLength(100);
            builder.Property(c => c.Title).IsRequired(false).HasMaxLength(100);
        }
    }
}
