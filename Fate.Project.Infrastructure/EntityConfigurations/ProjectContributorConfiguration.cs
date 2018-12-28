using Fate.Project.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.EntityConfigurations
{
    public class ProjectContributorConfiguration : IEntityTypeConfiguration<Project.Domain.AggregatesModel.ProjectContributor>
    {
        public void Configure(EntityTypeBuilder<ProjectContributor> builder)
        {
            builder.ToTable("ProjectContributor")
                .HasKey(c => c.Id);
        }
    }
}
