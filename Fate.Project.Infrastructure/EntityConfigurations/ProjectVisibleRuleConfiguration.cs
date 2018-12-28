using Fate.Project.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.EntityConfigurations
{
    public class ProjectVisibleRuleConfiguration : IEntityTypeConfiguration<Project.Domain.AggregatesModel.PrjectVisibleRule>
    {
        public void Configure(EntityTypeBuilder<PrjectVisibleRule> builder)
        {
            builder.ToTable("PrjectVisibleRule")
                .HasKey(c => c.Id);

        }
    }
}
