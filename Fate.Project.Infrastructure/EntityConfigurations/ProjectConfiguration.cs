using Fate.Project.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.EntityConfigurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project.Domain.AggregatesModel.Project>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.Project> builder)
        {
            builder.ToTable("Project")
                .HasKey(c => c.Id);
            builder.Property(c => c.Id).IsRequired(true).HasColumnName(@"Id").HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(c => c.UserId).IsRequired(false).HasColumnName(@"UserId");
            builder.Property(c => c.Avatar).IsRequired(false).HasColumnName(@"Avatar").HasMaxLength(100);
            builder.Property(c => c.Company).IsRequired(false).HasMaxLength(100);
            builder.Property(c => c.OriginBPFile).IsRequired(false).HasMaxLength(100);
            builder.Property(c => c.FormatBPFile).IsRequired(false).HasMaxLength(100);
            builder.Property(c => c.ShowSecurityInfo).IsRequired(false);
            builder.Property(c => c.ProvinceId).IsRequired(false);
            builder.Property(c => c.Province).IsRequired(false).HasMaxLength(50);
            builder.Property(c => c.CityId).IsRequired(false);
            builder.Property(c => c.City).IsRequired(false).HasMaxLength(50);
            builder.Property(c => c.AreaId).IsRequired(false);
            builder.Property(c => c.AreaName).IsRequired(false).HasMaxLength(50);
            builder.Property(c => c.RegisterTime).IsRequired(false);
            builder.Property(c => c.Introduction).IsRequired(false).HasMaxLength(500);
            builder.Property(c => c.FinPercentage).IsRequired(false).HasMaxLength(50);
            builder.Property(c => c.FinStage).IsRequired(false).HasMaxLength(50);
            builder.Property(c => c.Tags).IsRequired(false).HasMaxLength(500);
        }
    }
}
