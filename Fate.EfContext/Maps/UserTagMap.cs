using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Users;

namespace Fate.EfContext.Maps
{
    public class UserTagMap : IEntityTypeConfiguration<UserTags>
    {
        public void Configure(EntityTypeBuilder<UserTags> builder)
        {
            builder.ToTable("UserTags")
                .HasKey(c => new { c.UserId,c.Tag});
            builder.Property(c => c.UserId).IsRequired(true).HasColumnName(@"UserId").HasColumnType("int");
            builder.Property(c => c.Tag).IsRequired(true).HasColumnName(@"Tag").HasMaxLength(20);
            builder.Property(c => c.CreateTime).IsRequired(false).HasColumnName(@"datetime");

        }
    }
}
