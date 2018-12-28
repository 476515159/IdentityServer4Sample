using Fate.Project.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.EntityConfigurations
{
    public class ProjectViewerConfiguration : IEntityTypeConfiguration<Project.Domain.AggregatesModel.ProjectViewer>
    {
        public void Configure(EntityTypeBuilder<ProjectViewer> builder)
        {
            builder.ToTable("ProjectViewer")
                .HasKey(c => c.Id);
        }
    }
}
