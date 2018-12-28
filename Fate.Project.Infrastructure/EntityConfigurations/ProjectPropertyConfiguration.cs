using Fate.Project.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.EntityConfigurations
{
    public class ProjectPropertyConfiguration: IEntityTypeConfiguration<Project.Domain.AggregatesModel.ProjectProperty>
    {
        public void Configure(EntityTypeBuilder<ProjectProperty> builder)
        {
            builder.ToTable("ProjectProperty")
                .HasKey(c => new { c.Key,c.Value,c.ProjectId});
            builder.Property(c => c.ProjectId).IsRequired();
            builder.Property(c => c.Key).HasMaxLength(100);
            builder.Property(c => c.Value).HasMaxLength(100);
        }
    }
}
